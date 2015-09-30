using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace VinjEx
{
    class Util
    {
        #region Fixed methods from Easyhook for two-way communication

        internal static IpcChannel IpcCreateServer<TRemoteObject>(
               ref String RefChannelName,
               WellKnownObjectMode InObjectMode,
               TRemoteObject ipcInterface,
               params WellKnownSidType[] InAllowedClientSIDs) where TRemoteObject : MarshalByRefObject
        {
            String ChannelName = RefChannelName ?? GenerateName();

            ///////////////////////////////////////////////////////////////////
            // create security descriptor for IpcChannel...
            System.Collections.IDictionary Properties = new System.Collections.Hashtable();

            Properties["name"] = ChannelName;
            Properties["portName"] = ChannelName;

            DiscretionaryAcl DACL = new DiscretionaryAcl(false, false, 1);

            if (InAllowedClientSIDs.Length == 0)
            {
                if (RefChannelName != null)
                    throw new System.Security.HostProtectionException("If no random channel name is being used, you shall specify all allowed SIDs.");

                // allow access from all users... Channel is protected by random path name!
                DACL.AddAccess(
                    AccessControlType.Allow,
                    new SecurityIdentifier(
                        WellKnownSidType.WorldSid,
                        null),
                    -1,
                    InheritanceFlags.None,
                    PropagationFlags.None);
            }
            else
            {
                for (int i = 0; i < InAllowedClientSIDs.Length; i++)
                {
                    DACL.AddAccess(
                        AccessControlType.Allow,
                        new SecurityIdentifier(
                            InAllowedClientSIDs[i],
                            null),
                        -1,
                        InheritanceFlags.None,
                        PropagationFlags.None);
                }
            }

            CommonSecurityDescriptor SecDescr = new CommonSecurityDescriptor(false, false,
                ControlFlags.GroupDefaulted |
                ControlFlags.OwnerDefaulted |
                ControlFlags.DiscretionaryAclPresent,
                null, null, null,
                DACL);

            //////////////////////////////////////////////////////////
            // create IpcChannel...
            BinaryClientFormatterSinkProvider BinaryClient = new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider BinaryProv = new BinaryServerFormatterSinkProvider();
            BinaryProv.TypeFilterLevel = TypeFilterLevel.Full;

            IpcChannel Result = new IpcChannel(Properties, BinaryClient, BinaryProv);

            ChannelServices.RegisterChannel(Result, false);

            if (ipcInterface == null)
            {
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(TRemoteObject),
                    ChannelName,
                    InObjectMode);
            }
            else
            {
                RemotingServices.Marshal(ipcInterface, ChannelName);
            }

            RefChannelName = ChannelName;

            return Result;
        }

        internal static String GenerateName()
        {
            RNGCryptoServiceProvider Rnd = new RNGCryptoServiceProvider();
            Byte[] Data = new Byte[30];
            StringBuilder Builder = new StringBuilder();

            Rnd.GetBytes(Data);

            for (int i = 0; i < (20 + (Data[0] % 10)); i++)
            {
                Byte b = (Byte)(Data[i] % 62);

                if ((b >= 0) && (b <= 9))
                    Builder.Append((Char)('0' + b));
                if ((b >= 10) && (b <= 35))
                    Builder.Append((Char)('A' + (b - 10)));
                if ((b >= 36) && (b <= 61))
                    Builder.Append((Char)('a' + (b - 36)));
            }

            return Builder.ToString();
        }
        #endregion

    }
}
