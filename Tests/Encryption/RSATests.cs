using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Elasmobranch.Encryption;
using NUnit.Framework;

namespace Tests.Encryption
{
    public class RSATests
    {
        [Test]
        public void PrimeGenerationTest()
        {
            Assert.AreEqual(new List<BigInteger> {2, 3, 5, 7, 11, 13, 17, 19},
                RivestShamirAdleman.GenerateListOfPrimes(20));
        }

        [Test]
        [TestCase("13", "13", "13")]
        [TestCase("1", "37", "600")]
        [TestCase("20", "100", "20")]
        [TestCase("18913", "624129", "2061517")]
        [TestCase("526", "24826148", "45296490")]
        [TestCase("12", "12", "0")]
        [TestCase("0", "0", "0")]
        [TestCase("32", "7966496", "314080416")]
        [TestCase("18", "461952", "116298")]
        [TestCase("2", "4095484568135646548", "9014548534231345454")]
        public void GreatestCommonDivisorTest(string expected, string a, string b)
        {
            Assert.AreEqual(BigInteger.Parse(expected),
                RivestShamirAdleman.GreatestCommonDivisor(BigInteger.Parse(a), BigInteger.Parse(b)));
        }

        [Test]
        [TestCase("780", "60", "52")]
        [TestCase("41484157651764614525905399263631111992263435437186260", "234516789234023485693020129",
            "176892058718950472893785940")]
        [TestCase("443593541011902763984944550799004089258248037004507648321189937329",
            "36594652830916364940473625749407", "448507083624364748494746353648484939")]
        public void LeastCommonMultipleTest(string expected, string a, string b)
        {
            Assert.AreEqual(BigInteger.Parse(expected),
                RivestShamirAdleman.LeastCommonMultiple(BigInteger.Parse(a), BigInteger.Parse(b)));
        }

        [Test]
        [TestCase("413", "17", "780")]
        [TestCase("25", "7", "87")]
        [TestCase("1", "1", "2")]
        [TestCase("7", "25", "87")]
        [TestCase("46", "2", "91")]
        [TestCase("701912218", "19", "1212393831")]
        [TestCase("45180085378", "31", "73714876143")]
        public void ModularMultiplicativeInverseTest(string expected, string a, string m)
        {
            Assert.AreEqual(BigInteger.Parse(expected),
                RivestShamirAdleman.ModularMultiplicativeInverse(BigInteger.Parse(a), BigInteger.Parse(m)));
        }

        [Test]
        [TestCase(2790, 17, 3233, 65)]
        public void RSANumberEncryptionTest(int encrypted, int publicKey, int commonKey, int message)
        {
            // private key = -1, we won't need it for encryption!
            var rsaKey = new RivestShamirAdleman(-1, publicKey, commonKey);
            Assert.AreEqual(new BigInteger(encrypted), rsaKey.Encrypt(message));
        }

        [Test]
        [TestCase(65, 413, 3233, 2790)]
        public void RSANumberDecryptionTest(int message, int privateKey, int commonKey, int encrypted)
        {
            // public key = -1, we won't need it for decryption!
            var rsaKey = new RivestShamirAdleman(privateKey, -1, commonKey);
            Assert.AreEqual(new BigInteger(message), rsaKey.Decrypt(encrypted));
        }

        /// <summary>
        /// Test that the key pairs generated by my RSA implementation are actually valid and work for both encryption and decryption
        /// In my testing it turned out that initially the keys I was generating were not actually a valid key pair, explaining
        /// why actually encrypting and decrypting text didn't work -- therefore I wrote this test
        /// </summary>
        [Test]
        [TestCase(3)]
        [TestCase(5)]
        // TODO Fix RSA algorithm for longer numbers
        // [TestCase(7)]
        // [TestCase(8)]
        // [TestCase(9)]
        // [TestCase(10)]
        // [TestCase(20)]
        // [TestCase(100)]
        public void RSAKeyPairGenerationTest(int messageLength)
        {
            for (var i = 0; i < 5; i++)
            {
                var rsaKey = new RivestShamirAdleman();
                for (var j = 0; j < 10; j++)
                {
                    var number = BigInteger.Parse(GenerateRandomNumberString(messageLength));
                    var encrypted = rsaKey.Encrypt(number);
                    var decrypted = rsaKey.Decrypt(encrypted);
                    TestContext.WriteLine(
                        $"privateKey: {rsaKey.PrivateKey}, publicKey: {rsaKey.PublicKey}, commonKey: {rsaKey.CommonKey}");
                    TestContext.WriteLine($"input: {number}, encrypted: {encrypted}, decrypted: {decrypted}");
                    Assert.AreEqual(number, decrypted);
                }
            }
        }

        private static string GenerateRandomNumberString(int length = 20)
        {
            var digits = new[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};

            var stringBuilder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append(digits[random.Next(digits.Length)]);
            }

            return stringBuilder.ToString();
        }
        
        [Test]
        [Ignore("TODO Re-enable test once messages longer than a few digits are able to be successfully encrypted/decrypted")]
        public void RSATest()
        {
            var rsaKey = new RivestShamirAdleman();
        
            TestContext.WriteLine(
                $"privateKey: {rsaKey.PrivateKey}, publicKey: {rsaKey.PublicKey}, commonKey: {rsaKey.CommonKey}");
        
            var encrypted = rsaKey.Encrypt("topSecret123");
            TestContext.WriteLine(encrypted);
            Assert.AreEqual("topSecret123", rsaKey.Decrypt(encrypted));
        
            //Assert.AreEqual("topSecret123", rsaKey.PrivateDecrypt(rsaKey.PublicEncrypt("topSecret123")));
        }
    }
}