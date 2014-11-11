using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Cracker
{
    class Program
    {

        static char[] key,charset;
        static int index = 29,keysize = 4;
        static List<string> keys;
        static Dictionary<string, string> hash_table;
        static string key_prefix = "MeetInTheMiddleEncryptionKey";
        static void Initialize()
        {
            int i=0;char ch = '0';
            charset = new char[62];
            for (i = 0; i < 10; i++)
            {
                charset[i] = ch++;
            }

            ch = 'A';
            for (i = 0; i < 26; i++)
            {
                charset[10 + i] = ch++;
            }
            ch = 'a';
            for (i = 0; i < 26; i++)
            {
                charset[36 + i] = ch++;
            }

            key = new char[keysize];
            keys = new List<string>();
            hash_table = new Dictionary<string, string>();
        }

        //start pos with 0
        static void KeyGenerator(int pos)
        {
            if (pos == keysize)
            {
                keys.Add(new string(key));
                return;
            }
            for (int i = 0; i < charset.Length; i++)
            {
                if (pos == 0 && charset[i] >= 'a' && charset[i] <= 'z')
                {
                    continue;
                }
                else
                {
                    key[pos] = charset[i];
                    KeyGenerator(pos + 1);
                }
                
            }

        }

        static string UTFToB64(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }


        static string AsciiToUTF(string input)
        {
            return Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(input));
        }

        static string B64ToUTF(string input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }
        static string B64(string input)
        {
            return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(input));
        }

        static string Ascii(string inputB64)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(inputB64));
        }

        static string AES_Encrypt(string input, string key)
        {
            byte[] iv = new byte[16];
            Aes aes = new AesManaged();
            string cipher_text = String.Empty;
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.ECB;
            //aes.Key = Convert.FromBase64String(key);
            //aes.IV =new byte[16];
            aes.Padding = PaddingMode.None;
            var Encryptor = aes.CreateEncryptor(Encoding.ASCII.GetBytes(key),new byte[16]);
            using (MemoryStream cipher_stream = new MemoryStream())
            {
                using (CryptoStream plain_stream = new CryptoStream(cipher_stream, Encryptor, CryptoStreamMode.Write))
                {
                    byte[] input_bytes = Convert.FromBase64String(input),cipher_bytes;
                    plain_stream.Write(input_bytes, 0, input_bytes.Length);
                    plain_stream.FlushFinalBlock();
                  //  cipher_stream.Read(cipher_bytes, 0, 16);
                    cipher_bytes = cipher_stream.ToArray();
                    cipher_stream.Close();
                    plain_stream.Close();
                    
                    cipher_text = Convert.ToBase64String(cipher_bytes,0,cipher_bytes.Length);
                }
            }
            aes.Clear();
            return cipher_text;
        }


        static string AES_Decrypt(string input, string key)
        {

            Aes aes = new AesManaged();
            string plain_text = String.Empty;
            aes.BlockSize = 128;
            aes.KeySize = 256;
           
            //aes.Key = Convert.FromBase64String(key);
            //aes.IV = new byte[16];
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            var Decryptor = aes.CreateDecryptor(Encoding.ASCII.GetBytes(key),new byte[16]);
            using (MemoryStream plain_stream = new MemoryStream())
            {
                using (CryptoStream cipher_stream = new CryptoStream(plain_stream, Decryptor, CryptoStreamMode.Write))
                {
                    byte[] cipher_bytes = Convert.FromBase64String(input),plain_bytes ;
                    cipher_stream.Write(cipher_bytes, 0, cipher_bytes.Length);
                    cipher_stream.FlushFinalBlock();
                    //plain_stream.Read(plain_bytes, 0, 16);
                    plain_bytes = plain_stream.ToArray();
                    plain_stream.Close();
                    cipher_stream.Close();
                    plain_text = Convert.ToBase64String(plain_bytes, 0, plain_bytes.Length);

                }
            }
            aes.Clear();
            return plain_text;
        }

        static void Main(string[] args)
        {
            string input, ct1, ct2, pt1, pt2,key1,key2,im,inputb64,ct1b64,ct2b64,pt1b64,pt2b64,key1b64,key2b64 , u1,u2;



            input = "Hello World!!!!!,My Name is Neo!";
            key1 = "MeetInTheMiddleEncryptionKeyA93a";
            key2 = "MeetInTheMiddleEncryptionKeyP1Qs";

          

            inputb64 =B64(input);
            key1b64 = B64(key1);
            key2b64 = B64(key2);

          

            ct1b64 = AES_Encrypt(inputb64, key1);
            im = AES_Decrypt(ct1b64, key1);
            ct1 = Ascii(im);
            u2 = B64ToUTF(im);
            ct2b64 = AES_Encrypt(ct1b64, key2);

            pt2b64 = AES_Decrypt(ct2b64, key2);
            pt1b64 = AES_Decrypt(pt2b64, key1);

            pt1 = Ascii(pt1b64);

            //cracking

            Initialize();
            KeyGenerator(0);

            foreach (string key in keys)
            {
                string encrypted_text = AES_Encrypt(inputb64,key_prefix + key);
                    hash_table.Add(encrypted_text, key);
                
                
            }

            foreach (string key in keys)
            {
                string decrypted_text = AES_Decrypt(ct2b64,key_prefix + key);
                if (hash_table.ContainsKey(decrypted_text))
                {
                    Console.WriteLine(String.Format("KEY1:{0}\nKEY2:{1}",key_prefix + hash_table[decrypted_text],key_prefix + key));
                    break;
                }
            }

            




            Console.Read();


        }
    }
}
