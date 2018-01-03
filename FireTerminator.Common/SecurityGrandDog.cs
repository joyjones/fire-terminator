using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FireTerminator.Common
{
    public class SecurityGrandDog
    {
        private static SecurityGrandDog m_Instance;
        public static SecurityGrandDog Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SecurityGrandDog();
                return m_Instance;
            }
        }
        public uint LastErrorCode
        {
            get;
            private set;
        }
        private uint DogHandle = 0;

        //define the import function  ,CallingConvention=CallingConvention.Cdecl 
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_OpenDog(uint ulFlag, byte* pszProductName, uint* pDogHandle);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_GetDogInfo(uint DogHandle, byte* pHardwareInfo, uint* pulLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_GetProductCurrentNo(uint DogHandle, uint* pulProductCurrentNo);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_VerifyPassword(uint DogHandle, byte bPasswordType, string szPassword, byte* pbDegree);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_ChangePassword(uint DogHandle, byte bPasswordType, string szPassword);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_SetKey(uint DogHandle, byte bKeyType, byte* pucIn, uint ulLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_EncryptData(uint DogHandle, byte* pucIn, uint ulInLen, byte* pucOut, uint* pulOutLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_DecryptData(uint DogHandle, byte* pucIn, uint ulInLen, byte* pucOut, uint* pulOutLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_SignData(uint DogHandle, byte* pucIn, uint ulInLen, byte* pucOut, uint* pulOutLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_ConvertData(uint DogHandle, byte* pucIn, uint ulInLen, uint* pulResult);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_CheckDog(uint DogHandle);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_GetRandom(uint DogHandle, byte* pucOut, uint ulInLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_CreateDir(uint DogHandle, ushort usDirID, uint ulDirSize);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_CreateFile(uint DogHandle, ushort usDirID, ushort usFileID, byte bFiletype, uint ulFileSize);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_DeleteDir(uint DogHandle, ushort usDirID);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_DeleteFile(uint DogHandle, ushort usDirID, ushort usFileID);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_DefragFileSystem(uint DogHandle, ushort usDirID);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_ReadFile(uint DogHandle, ushort usDirID, ushort usFileID, uint ulPos, uint ulLen, byte* pucOut);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_WriteFile(uint DogHandle, ushort usDirID, ushort usFileID, uint ulPos, uint ulLen, byte* pucIn);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_VisitLicenseFile(uint DogHandle, ushort usDirID, ushort usFileID, uint ulReserved);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_ExecuteFile(uint DogHandle, ushort usDirID, ushort usFileID, byte* pucIn, uint ulInlen, byte* pucOut, uint* pulOutlen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_GetUpgradeRequestString(uint DogHandle, byte* pucBuf, uint* pulLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_Upgrade(uint DogHandle, byte* pucUpgrade, uint ulLen);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_CloseDog(uint DogHandle);
        [DllImport("RCGrandDogW32.dll", CharSet = CharSet.Ansi)]
        public static unsafe extern uint rc_GetLicenseInfo(uint DogHandle, ushort usDirID, ushort usFileID, ushort* pusLimit, uint* pulCount, uint* pulRuntime,
            ushort* pusBeginYear, byte* pbBeginMonth, byte* pbBeginDay, byte* pbBeginHour, byte* pbBeginMinute, byte* pbBeginSecond,
            ushort* pusEndYear, byte* pbEndMonth, byte* pbEndDay, byte* pbEndHour, byte* pbEndMinute, byte* pbEndSecond);


        //const size for validation
        public static readonly uint RC_OPEN_FIRST_IN_LOCAL = 1;
        public static readonly uint RC_OPEN_NEXT_IN_LOCAL = 2;
        public static readonly uint RC_OPEN_IN_LAN = 3;
        public static readonly uint RC_OPEN_LOCAL_FIRST = 4;
        public static readonly uint RC_OPEN_LAN_FIRST = 5;

        public static readonly byte RC_PASSWORDTYPE_USER = 1;
        public static readonly byte RC_PASSWORDTYPE_DEVELOPER = 2;

        public static readonly byte RC_DOGTYPE_LOCAL = 1;
        public static readonly byte RC_DOGTYPE_NET = 2;

        public static readonly byte RC_TYPEFILE_DATA = 1;
        public static readonly byte RC_TYPEFILE_LICENSE = 2;
        public static readonly byte RC_TYPEFILE_ALGORITHMS = 3;

        public static readonly byte RC_KEY_AES = 1;
        public static readonly ulong E_RC_VERIFY_PASSWORD_FAILED = 0xA816000C;
        public static readonly byte RC_KEY_SIGN = 2;

        public bool StartupCheck()
        {
#if !DEBUG
            while(!Open())
            {
                if (DialogResult.Cancel == MessageBox.Show("请插入加密狗！", "未检测到加密狗", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error))
                    return false;
            }
            if (!RunningCheck())
                return false;
#endif
            return true;
        }
        public bool RunningCheck()
        {
#if !DEBUG
            if (DogHandle == 0)
                return false;
            while (!VerifyPassword())
            {
                if (DialogResult.Cancel == MessageBox.Show("请插入正确的加密狗！", "加密狗用户口令校验失败", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error))
                    return false;
                while (!Open())
                {
                    if (DialogResult.Cancel == MessageBox.Show("请插入加密狗！", "未检测到加密狗", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error))
                        return false;
                }
            }
#endif
            return true;
        }

        public unsafe bool VerifyPassword()
        {
            byte bDegree;
            LastErrorCode = rc_VerifyPassword(DogHandle, RC_PASSWORDTYPE_USER, "wujingxueyuan@langfang", &bDegree);
            return LastErrorCode == 0;
        }

        public unsafe bool Open()
        {
            char[] product = "GrandDog".ToCharArray();
            fixed (uint* pDogHandle = &DogHandle)
            {
                fixed (byte* pProductName = new byte[16])
                {
                    for (int i = 0; i < product.Length; i++)
                        *(pProductName + i) = (byte)(product[i]);
                    *(pProductName + product.Length) = 0;
                    LastErrorCode = rc_OpenDog(RC_OPEN_FIRST_IN_LOCAL, pProductName, pDogHandle);
                    return LastErrorCode == 0;
                }
            }
        }

        public void Close()
        {
            if (DogHandle != 0)
                rc_CloseDog(DogHandle);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        public unsafe uint GetDogInfo(uint DogHandle, byte* pHardwareInfo, uint* pulLen)
        {
            return rc_GetDogInfo(DogHandle, pHardwareInfo, pulLen);
        }

        public unsafe uint GetProductCurrentNo(uint DogHandle, uint* pulProductCurrentNo)
        {
            return rc_GetProductCurrentNo(DogHandle, pulProductCurrentNo);
        }

        public unsafe uint ChangePassword(uint DogHandle, byte bPasswordType, string szPassword)
        {
            return rc_ChangePassword(DogHandle, bPasswordType, szPassword);
        }

        public unsafe uint SetKey(uint DogHandle, byte bKeyType, byte* pucIn, uint ulLen)
        {
            return rc_SetKey(DogHandle, bKeyType, pucIn, ulLen);
        }

        public unsafe uint EncryptData(uint DogHandle, byte* pucIn, uint ulInLen, byte* pucOut, uint* pulOutLen)
        {
            return rc_EncryptData(DogHandle, pucIn, ulInLen, pucOut, pulOutLen);
        }

        public unsafe uint DecryptData(uint DogHandle, byte* pucIn, uint ulInLen, byte* pucOut, uint* pulOutLen)
        {
            return rc_DecryptData(DogHandle, pucIn, ulInLen, pucOut, pulOutLen);
        }

        public unsafe uint SignData(uint DogHandle, byte* pucIn, uint ulInLen, byte* pucOut, uint* pulOutLen)
        {
            return rc_SignData(DogHandle, pucIn, ulInLen, pucOut, pulOutLen);
        }

        public unsafe uint ConvertData(uint DogHandle, byte* pucIn, uint ulInLen, uint* pulResult)
        {
            return rc_ConvertData(DogHandle, pucIn, ulInLen, pulResult);
        }

        public unsafe uint CheckDog(uint DogHandle)
        {
            return rc_CheckDog(DogHandle);
        }

        public unsafe uint GetRandom(uint DogHandle, byte* pucOut, uint ulInLen)
        {
            return rc_GetRandom(DogHandle, pucOut, ulInLen);
        }

        public unsafe uint CreateDir(uint DogHandle, ushort usDirID, uint ulDirSize)
        {
            return rc_CreateDir(DogHandle, usDirID, ulDirSize);
        }

        public unsafe uint CreateFile(uint DogHandle, ushort usDirID, ushort usFileID, byte bFiletype, uint ulFileSize)
        {
            return rc_CreateFile(DogHandle, usDirID, usFileID, bFiletype, ulFileSize);
        }

        public unsafe uint DeleteDir(uint DogHandle, ushort usDirID)
        {
            return rc_DeleteDir(DogHandle, usDirID);
        }

        public unsafe uint DeleteFile(uint DogHandle, ushort usDirID, ushort usFileID)
        {
            return rc_DeleteFile(DogHandle, usDirID, usFileID);
        }

        public unsafe uint DefragFileSystem(uint DogHandle, ushort usDirID)
        {
            return rc_DefragFileSystem(DogHandle, usDirID);
        }

        public unsafe uint ReadFile(uint DogHandle, ushort usDirID, ushort usFileID, uint ulPos, uint ulLen, byte* pucOut)
        {
            return rc_ReadFile(DogHandle, usDirID, usFileID, ulPos, ulLen, pucOut);
        }

        public unsafe uint WriteFile(uint DogHandle, ushort usDirID, ushort usFileID, uint ulPos, uint ulLen, byte* pucIn)
        {
            return rc_WriteFile(DogHandle, usDirID, usFileID, ulPos, ulLen, pucIn);
        }

        public unsafe uint VisitLicenseFile(uint DogHandle, ushort usDirID, ushort usFileID, uint ulReserved)
        {
            return rc_VisitLicenseFile(DogHandle, usDirID, usFileID, ulReserved);
        }

        public unsafe uint ExecuteFile(uint DogHandle, ushort usDirID, ushort usFileID, byte* pucIn, uint ulInlen, byte* pucOut, uint* pulOutlen)
        {
            return rc_ExecuteFile(DogHandle, usDirID, usFileID, pucIn, ulInlen, pucOut, pulOutlen);
        }

        public unsafe uint GetUpgradeRequestString(uint DogHandle, byte* pucBuf, uint* pulLen)
        {
            return rc_GetUpgradeRequestString(DogHandle, pucBuf, pulLen);
        }

        public unsafe uint Upgrade(uint DogHandle, byte* pucUpgrade, uint pulLen)
        {
            return rc_Upgrade(DogHandle, pucUpgrade, pulLen);
        }
        public unsafe uint GetLicenseInfo(uint DogHandle, ushort usDirID, ushort usFileID, ushort* pusLimit, uint* pulCount, uint* pulRuntime,
            ushort* pusBeginYear, byte* pbBeginMonth, byte* pbBeginDay, byte* pbBeginHour, byte* pbBeginMinute, byte* pbBeginSecond,
            ushort* pusEndYear, byte* pbEndMonth, byte* pbEndDay, byte* pbEndHour, byte* pbEndMinute, byte* pbEndSecond)
        {
            return rc_GetLicenseInfo(DogHandle, usDirID, usFileID, pusLimit, pulCount, pulRuntime,
                pusBeginYear, pbBeginMonth, pbBeginDay, pbBeginHour, pbBeginMinute, pbBeginSecond,
                pusEndYear, pbEndMonth, pbEndDay, pbEndHour, pbEndMinute, pbEndSecond);
        }
    }
}
