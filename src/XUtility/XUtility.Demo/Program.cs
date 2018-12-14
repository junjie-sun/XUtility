using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XUtility.Security;
using XUtility.Threading;

namespace XUtility.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            #region CryptographyUtils

            //MD5EncryptDemo();

            //GetFileMD5HashDemo();

            //SHA1EncryptDemo();

            //GetFileSHA1HashDemo();

            //SHA256EncryptDemo();

            //SHA384EncryptDemo();

            //SHA512EncryptDemo();

            //AESEncryptDemo();

            //RSAEncryptDemo();

            //PhoneMaskDemo();

            //FullNameMaskDemo();

            #endregion

            #region SignatureUtils

            //GenerateSignatureDemo();

            //GenerateSignatureWithEncryptPwdDemo();

            //ValidateSignatureDemo();

            //ValidateSignatureDemo2();

            #endregion

            #region ThreadingTest

            //var t = WithCancellationTest();

            //AsyncOneManyLockTest();

            #endregion

            #region Notice

            //NoticeDemo.PublishSubscribeDemo();
            //NoticeDemo.ReliablePublishSubscribeDemo();
            //NoticeDemo.NoticeManagerDefaultDemo();
            //NoticeDemo.NoticeManagerSequenceDemo();
            //NoticeDemo.NoticeManagerDefaultStackDemo();
            //NoticeDemo.NoticeManagerSequenceStopDemo();
            //NoticeDemo.NoticeManagerParallelStopDemo();
            //NoticeDemo.NoticeManagerLargeNumberDemo();

            #endregion

            Console.ReadLine();
        }

        #region CryptographyUtils

        private static void MD5EncryptDemo()
        {
            var original = "This is MD5 Encrypt Demo.";
            var result = CryptographyUtils.MD5Encrypt(original);
            Console.WriteLine(result);
            if (result.Equals("7c243f5af73c7ff5ec7fec10f45172e9", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("true");
            }
        }

        private static void GetFileMD5HashDemo()
        {
            var path = System.IO.Directory.GetCurrentDirectory() + @"\Demo.png";
            var md5 = CryptographyUtils.GetFileMD5Hash(path);
            Console.WriteLine(md5);
            if (md5.Equals("c7862d93618a4abe90c708a4b0d01ef3", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("true");
            }
        }

        private static void SHA1EncryptDemo()
        {
            var original = "This is SHA1 Encrypt Demo.";
            var result = CryptographyUtils.SHA1Encrypt(original);
            Console.WriteLine(result);
            if (result.Equals("c6a7de180d28dd6825e6e0eab80ac8ab5fe4eb7f", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("true");
            }
        }

        private static void GetFileSHA1HashDemo()
        {
            var path = System.IO.Directory.GetCurrentDirectory() + @"\Demo.png";
            var sha1 = CryptographyUtils.GetFileSHA1Hash(path);
            Console.WriteLine(sha1);
            if (sha1.Equals("3ca789fcc7cd395314e0a7a8f49775bbd3ce334a", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("true");
            }
        }

        private static void SHA256EncryptDemo()
        {
            var original = "This is SHA256 Encrypt Demo.";
            var result = CryptographyUtils.SHA256Encrypt(original);
            Console.WriteLine(result);
            if (result.Equals("b3944e910db6b876255bf7642b119e457599e5642b92a0262a9b2151c99cf822", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("true");
            }
        }

        private static void SHA384EncryptDemo()
        {
            var original = "This is SHA384 Encrypt Demo.";
            var result = CryptographyUtils.SHA384Encrypt(original);
            Console.WriteLine(result);
            if (result.Equals("5c65a66c1e25be123218bf7ba7c9851a8946efb2c51eab64c71550d5415654415ab7a629089d13082c99af6b2b971e0e", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("true");
            }
        }

        private static void SHA512EncryptDemo()
        {
            var original = "This is SHA512 Encrypt Demo.";
            var result = CryptographyUtils.SHA512Encrypt(original);
            Console.WriteLine(result);
            if (result.Equals("efe8e6e4ad882f89fbff3e980d956689f54667eff92213179b9b5f20a5e4dfb185b5d896870bf0a2550930e63225e099aee49d52eb7b929c900f56c35e5e36d8", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("true");
            }
        }

        private static void AESEncryptDemo()
        {
            var original = "This is AES Encrypt Demo.";
            var password = "Ki)5McIoe9&G?a$P";
            var encryptText = CryptographyUtils.AESEncrypt(original, password);
            Console.WriteLine(encryptText);
            var decryptText = CryptographyUtils.AESDecrypt(encryptText, password);
            Console.WriteLine(decryptText);
        }

        private static void RSAEncryptDemo()
        {
            var original = "This is RSA Encrypt Demo.";
            var privateKey = @"MIICXgIBAAKBgQC0xP5HcfThSQr43bAMoopbzcCyZWE0xfUeTA4Nx4PrXEfDvybJ
EIjbU/rgANAty1yp7g20J7+wVMPCusxftl/d0rPQiCLjeZ3HtlRKld+9htAZtHFZ
osV29h/hNE9JkxzGXstaSeXIUIWquMZQ8XyscIHhqoOmjXaCv58CSRAlAQIDAQAB
AoGBAJtDgCwZYv2FYVk0ABw6F6CWbuZLUVykks69AG0xasti7Xjh3AximUnZLefs
iuJqg2KpRzfv1CM+Cw5cp2GmIVvRqq0GlRZGxJ38AqH9oyUa2m3TojxWapY47zye
PYEjWwRTGlxUBkdujdcYj6/dojNkm4azsDXl9W5YaXiPfbgJAkEA4rlhSPXlohDk
FoyfX0v2OIdaTOcVpinv1jjbSzZ8KZACggjiNUVrSFV3Y4oWom93K5JLXf2mV0Sy
80mPR5jOdwJBAMwciAk8xyQKpMUGNhFX2jKboAYY1SJCfuUnyXHAPWeHp5xCL2UH
tjryJp/Vx8TgsFTGyWSyIE9R8hSup+32rkcCQBe+EAkC7yQ0np4Z5cql+sfarMMm
4+Z9t8b4N0a+EuyLTyfs5Dtt5JkzkggTeuFRyOoALPJP0K6M3CyMBHwb7WsCQQCi
TM2fCsUO06fRQu8bO1A1janhLz3K0DU24jw8RzCMckHE7pvhKhCtLn+n+MWwtzl/
L9JUT4+BgxeLepXtkolhAkEA2V7er7fnEuL0+kKIjmOm5F3kvMIDh9YC1JwLGSvu
1fnzxK34QwSdxgQRF1dfIKJw73lClQpHZfQxL/2XRG8IoA==".Replace("\n", "");
            var publicKey = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC0xP5HcfThSQr43bAMoopbzcCy
ZWE0xfUeTA4Nx4PrXEfDvybJEIjbU/rgANAty1yp7g20J7+wVMPCusxftl/d0rPQ
iCLjeZ3HtlRKld+9htAZtHFZosV29h/hNE9JkxzGXstaSeXIUIWquMZQ8XyscIHh
qoOmjXaCv58CSRAlAQIDAQAB".Replace("\n", "");
            var encryptText = CryptographyUtils.RSAEncrypt(original, publicKey);
            Console.WriteLine(encryptText);
            var decryptText = CryptographyUtils.RSADecrypt(encryptText, privateKey);
            Console.WriteLine(decryptText);
        }

        private static void PhoneMaskDemo()
        {
            var phone = "13012345678";
            var mask = CryptographyUtils.PhoneMask(phone);
            Console.WriteLine(mask);
        }

        private static void FullNameMaskDemo()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //解决中文乱码
            var encoding = Encoding.GetEncoding("GBK");
            var name = "张三丰";
            var mask = CryptographyUtils.FullNameMask(name);
            Console.WriteLine(mask);
        }

        #endregion

        #region SignatureUtils

        private static void GenerateSignatureDemo()
        {
            var pwd = "123456";
            var queryStringList = new Dictionary<string, string>();
            var timestamp = SignatureUtils.GetTimestamp().ToString();

            queryStringList.Add("merchant_name", "testaccount");
            queryStringList.Add("signature_method", "SHA1");
            queryStringList.Add("timestamp", timestamp);
            queryStringList.Add("method", "testmethod");
            queryStringList.Add("phone", "13012345678");

            var sign = SignatureUtils.GenerateSignature(queryStringList, pwd, "&");

            queryStringList.Add("signature", sign);

            Console.WriteLine("timestamp=" + timestamp);
            Console.WriteLine("signature=" + sign);
        }

        private static void GenerateSignatureWithEncryptPwdDemo()
        {
            var pwd = "123456";
            var queryStringList = new Dictionary<string, string>();
            var timestamp = SignatureUtils.GetTimestamp().ToString();

            queryStringList.Add("merchant_name", "testaccount");
            queryStringList.Add("signature_method", "SHA1");
            queryStringList.Add("timestamp", timestamp);
            queryStringList.Add("method", "testmethod");
            queryStringList.Add("phone", "13012345678");

            var sign = SignatureUtils.GenerateSignature(queryStringList, pwd, "&", new SignatureOptions()
            {
                IsEncryptPwd = true,
                PwdEncryptMode = EncryptMode.SHA256
            });

            Console.WriteLine("signature=" + sign);

            var sign2 = SignatureUtils.GenerateSignature(queryStringList, CryptographyUtils.SHA256Encrypt(pwd), "&");
            Console.WriteLine("signature=" + sign2);
        }

        private static void ValidateSignatureDemo()
        {
            var pwd = "123456";
            var signatureParams = new Dictionary<string, string>();
            var timestamp = SignatureUtils.GetTimestamp().ToString();

            signatureParams.Add("merchant_name", "testaccount");
            signatureParams.Add("signature_method", "SHA1");
            signatureParams.Add("timestamp", timestamp);
            signatureParams.Add("method", "testmethod");
            signatureParams.Add("phone", "13012345678");

            var signOpt = new SignatureOptions()
            {
                IsEncryptPwd = true,
                PwdEncryptMode = EncryptMode.SHA256,
                Method = SignatureMethod.SHA1
            };

            var sign = SignatureUtils.GenerateSignature(signatureParams, pwd, "&", signOpt);

            Console.WriteLine("signature=" + sign);

            var result = SignatureUtils.ValidateSignature(sign, signatureParams, pwd, "&", new SignatureValidateOptions()
            {
                IsCheckTimestamp = true,
                TimestampError = new TimeSpan(0, 5, 0),
                TimestampParamName = "timestamp",
                SignatureOptions = signOpt
            });

            Console.WriteLine(result);
        }

        private static void ValidateSignatureDemo2()
        {
            var pwd = "123456";
            var signatureParams = new Dictionary<string, string>();
            var timestamp = SignatureUtils.GetTimestamp().ToString();

            signatureParams.Add("merchant_name", "testaccount");
            signatureParams.Add("signature_method", "SHA1");
            signatureParams.Add("timestamp", timestamp);
            signatureParams.Add("method", "testmethod");
            signatureParams.Add("phone", "13012345678");

            var signOpt = new SignatureOptions()
            {
                IsEncryptPwd = true,
                PwdEncryptMode = EncryptMode.SHA256,
                Method = SignatureMethod.SHA1
            };

            var signStr = $"merchant_name=testaccount&method=testmethod&phone=13012345678&signature_method=SHA1&timestamp={timestamp}{CryptographyUtils.SHA256Encrypt(pwd)}";
            var sign = CryptographyUtils.SHA1Encrypt(signStr);

            Console.WriteLine("signature=" + sign);

            var result = SignatureUtils.ValidateSignature(sign, signatureParams, pwd, "&", new SignatureValidateOptions()
            {
                IsCheckTimestamp = true,
                TimestampError = new TimeSpan(0, 5, 0),
                TimestampParamName = "timestamp",
                SignatureOptions = signOpt
            });

            Console.WriteLine(result);
        }

        #endregion

        #region ThreadingTest

        private static async Task WithCancellationTest()
        {
            //创建一个CancellationTokenSource，它在#毫秒后取消自己
            var cts = new CancellationTokenSource(5000);    //更快取消需调用cts.Cancel()
            var ct = cts.Token;

            try
            {
                await Task.Delay(10000).WithCancellation(ct);
                Console.WriteLine("Task completed");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Task cancelled");
            }
        }

        private static void AsyncOneManyLockTest()
        {
            AsyncOneManyLock m_lock = new AsyncOneManyLock();

            Task.Run(async () =>
            {
                await m_lock.WaitAsync(OneManyMode.Shared);
                Console.WriteLine("I'm a reader1");
                Thread.Sleep(3000);
                m_lock.Release();
            });

            Task.Run(async () =>
            {
                await m_lock.WaitAsync(OneManyMode.Shared);
                Console.WriteLine("I'm a reader2");
                Thread.Sleep(3000);
                m_lock.Release();
            });

            Thread.Sleep(1000);

            Task.Run(async () =>
            {
                await m_lock.WaitAsync(OneManyMode.Exclusive);
                Console.WriteLine("I'm a writer");
                Thread.Sleep(3000);
                m_lock.Release();
            });

            Thread.Sleep(500);

            Task.Run(async () =>
            {
                await m_lock.WaitAsync(OneManyMode.Shared);
                Console.WriteLine("I'm a reader3");
                Thread.Sleep(3000);
                m_lock.Release();
            });

            Task.Run(async () =>
            {
                await m_lock.WaitAsync(OneManyMode.Shared);
                Console.WriteLine("I'm a reader4");
                Thread.Sleep(3000);
                m_lock.Release();
            });
        }

        #endregion
    }
}
