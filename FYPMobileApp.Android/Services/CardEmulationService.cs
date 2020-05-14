using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Nfc.CardEmulators;
using Android.Nfc;
using Android.Nfc.Tech;
using Plugin.CurrentActivity;
using Plugin.Fingerprint.Abstractions;
using Xamarin.Forms;
using FYPMobileApp.Services;

namespace FYPMobileApp.Droid.Services
{
        [Service(Exported = true, Enabled = true, Permission = "android.permission.BIND_NFC_SERVICE"),
IntentFilter(new[] { "android.nfc.cardemulation.action.HOST_APDU_SERVICE" }, Categories = new[] { "android.intent.category.DEFAULT" }),
MetaData("android.nfc.cardemulation.host_apdu_service", Resource = "@xml/aid_list")]
        class CardEmulationService : HostApduService, NfcAdapter.IReaderCallback
        {

            public NfcReaderFlags READER_FLAGS = NfcReaderFlags.NfcA | NfcReaderFlags.SkipNdefCheck;
            private static readonly string TAG = "AccessCardReader";

            // Smartcard Application Identifier (AID) for our service - keeps communication between our applications and not other ones
            private static readonly string ACCESS_CARD_AID = "FF69696969";

            //Using IsoDep allows us to trancieve and exchange Application Data Units (APDUs)

            // IsoDep command header for selecting an AID.
            // Format: [Class | Instruction | Parameter 1 | Parameter 2]
            private const String SELECT_APDU_HEADER = "00A40400";

            // "OK" status word sent in response to SELECT AID command (0x9000)
            private static readonly byte[] SELECT_OK_SW = HexStringToByteArray("9000");

            // "UNKNOWN" status word sent in response to invalid APDU command (0x0000)
            private static readonly byte[] UNKNOWN_CMD_SW = HexStringToByteArray("0000");

            //"AUTH_FAILURE" status word sent when fingerprint is not validated
            private static readonly byte[] AUTH_FAILURE_CMD_SW = HexStringToByteArray("9804");

            private static readonly byte[] SELECT_APDU = BuildSelectApdu(ACCESS_CARD_AID);

            public override byte[] ProcessCommandApdu(byte[] commandApdu, Bundle extras)
            {
                Console.WriteLine("[CODE] Received APDU: " + ByteArrayToHexString(commandApdu));
                // If the APDU matches the SELECT AID command for this service,
                // send the loyalty card account number, followed by a SELECT_OK status trailer (0x9000)

                // Check if the values inside of commandApdu are the same as SELECT_APDU 
                bool arrayEquals;
                if (SELECT_APDU.Length == commandApdu.Length)
                {
                    arrayEquals = true;
                    for (int i = 0; i < SELECT_APDU.Length; i++)
                    {
                        if (SELECT_APDU[i] != commandApdu[i])
                        {
                            arrayEquals = false;
                            break;
                        }
                    }
                }
                else
                {
                    arrayEquals = false;
                }

                if (arrayEquals)
                {

                    if (!Xamarin.Forms.Application.Current.Properties.ContainsKey("AuthStatus"))
                    {
                    Device.BeginInvokeOnMainThread(async () => {
                        await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Please authenticate fingerprint", "OK");
                    });

                    Console.WriteLine("No AuthStatus");
                    return AUTH_FAILURE_CMD_SW;
                    }

                    //Get the Authentication and UserId properties from the global application properties
                    FingerprintAuthenticationResult authResult = Xamarin.Forms.Application.Current.Properties["AuthStatus"] as FingerprintAuthenticationResult;
                    string userId = Xamarin.Forms.Application.Current.Properties["UserId"] as string;


                    if (authResult.Authenticated)
                    {
                        byte[] payload = Encoding.UTF8.GetBytes(userId);
                        Console.WriteLine($"[CODE] Sending payload {payload}");
                        bool removalStatus = Xamarin.Forms.Application.Current.Properties.Remove("AuthStatus");
                        NavigationService service = new NavigationService();
                        
                        //Heavy assumption that we're on the NFC page for this function, because we *shouldn't* be anywhere else, as the hardware back button is disabled on the NFC page
                        Device.BeginInvokeOnMainThread(async () => {
                            service.returnToPrevious();
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Scan Success", "Scan completed, please check access point for information", "OK");
                        });

                    return ConcatArrays(payload, SELECT_OK_SW);
                    }
                    else
                    {
                        //HCE service is open from the start - we can't prevent that
                        //But we can prevent the transfer of data without prior authentication
                        Device.BeginInvokeOnMainThread(async () => {
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Not Authenticated", "Please authenticate fingerprint before tapping to access point", "OK");
                        });

                        Console.WriteLine("Fingerprint failure");

                        bool removalStatus = Xamarin.Forms.Application.Current.Properties.Remove("AuthStatus");
                        return AUTH_FAILURE_CMD_SW;
                    }

                }
                else
                {
                    return UNKNOWN_CMD_SW;
                }
            }

            public override void OnDeactivated(DeactivationReason reason)
            {

            }

            //Reusable function to take a standard string and convert into an array of bytes that can be sent
            private static byte[] HexStringToByteArray(string s)
            {
                int len = s.Length;
                if (len % 2 == 1)
                {
                    throw new ArgumentException("Hex string must have even number of characters");
                }
                byte[] data = new byte[len / 2]; //Allocate 1 byte per 2 hex characters
                for (int i = 0; i < len; i += 2)
                {
                    ushort val, val2;
                    // Convert each chatacter into an unsigned integer (base-16)
                    try
                    {
                        val = (ushort)Convert.ToInt32(s[i].ToString() + "0", 16);
                        val2 = (ushort)Convert.ToInt32("0" + s[i + 1].ToString(), 16);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    data[i / 2] = (byte)(val + val2);
                }
                return data;
            }

            public static string ByteArrayToHexString(byte[] bytes)
            {
                String s = "";
                for (int i = 0; i < bytes.Length; i++)
                {
                    s += bytes[i].ToString("X2");
                }
                return s;
            }
            
            //APDU methods require an array of bytes to specific each portion of the command
            public static byte[] BuildSelectApdu(string aid)
            {
                // Format: [CLASS | INSTRUCTION | PARAMETER 1 | PARAMETER 2 | LENGTH | DATA]
                return HexStringToByteArray(SELECT_APDU_HEADER + (aid.Length / 2).ToString("X2") + aid);
            }

            //Reusable function to concat the arrays of bytes that contain the APDU command and the payload
            public static byte[] ConcatArrays(byte[] first, params byte[][] rest)
            {
                int totalLength = first.Length;
                foreach (byte[] array in rest)
                {
                    totalLength += array.Length;
                }
                byte[] result = new byte[totalLength];
                first.CopyTo(result, 0);
                int offset = first.Length;
                foreach (byte[] array in rest)
                {
                    array.CopyTo(result, offset);
                    offset += array.Length;
                }
                return result;
            }

            public void StartListening()
            {
                Activity activity = CrossCurrentActivity.Current.Activity;

                NfcAdapter nfc = NfcAdapter.GetDefaultAdapter(activity);
                CardEmulationService callback = new CardEmulationService();
                if (nfc != null)
                {
                    Console.WriteLine("[CODE] Reader Started");
                    nfc.EnableReaderMode(activity, callback, READER_FLAGS, null);
                }
            }

            public void OnTagDiscovered(Tag tag)
            {
                IsoDep isoDep = IsoDep.Get(tag);

                if (isoDep != null)
                {
                    isoDep.Connect();
                }
            }
        }
}