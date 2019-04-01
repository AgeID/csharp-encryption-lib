extern alias DotNetFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetFramework.AgeID;

namespace AgeID.EncryptionHelper.Tests
{
	[TestClass]
    public class DotNetFrameworkTests
	{
		private const string alphaNumericRange = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private const string fullRange = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`-=[]\\;',./~!@#$%^&*()_+{}|:\"<>?";

		private const string DEFAULT_TEXT = "somePass";
		private const string DEFAULT_PASS = "someText";
		private const string DEFAULT_SALT = "Rj2/dM5XZ8QTuw/Z2RzjDQ==";

		[TestMethod]
		public void EmptyTextTest()
		{
			AssertEncryptionIsValid(string.Empty, DEFAULT_PASS, DEFAULT_SALT);
		}

		[TestMethod]
		public void NoSaltTest()
		{
			AssertEncryptionIsValid(DEFAULT_TEXT, DEFAULT_PASS);
		}

		[TestMethod]
		public void AlphanumericRangeTextTest()
		{
			AssertEncryptionIsValid(alphaNumericRange, DEFAULT_PASS, DEFAULT_SALT);
		}

		[TestMethod]
		public void AlphanumericRangePassTest()
		{
			AssertEncryptionIsValid(DEFAULT_TEXT, alphaNumericRange, DEFAULT_SALT);
		}

		[TestMethod]
		public void AlphanumericRangeSaltTest()
		{
			AssertEncryptionIsValid(DEFAULT_TEXT, DEFAULT_PASS, alphaNumericRange);
		}

		[TestMethod]
		public void FullRangeTextTest()
		{
			AssertEncryptionIsValid(fullRange, DEFAULT_PASS, DEFAULT_SALT);
		}

		[TestMethod]
		public void FullRangePassTest()
		{
			AssertEncryptionIsValid(DEFAULT_TEXT, fullRange, DEFAULT_SALT);
		}

		[TestMethod]
		public void FullRangeSaltTest()
		{
			AssertEncryptionIsValid(DEFAULT_TEXT, DEFAULT_PASS, fullRange);
		}

		private void AssertEncryptionIsValid(string clearText, string pass, string salt = null, APIVersion apiVersion = APIVersion.V2)
		{
			string encodedPayload = AgeIDEncryptionHelper.Encrypt(clearText, pass, salt, apiVersion);
			string decryptedText = AgeIDEncryptionHelper.Decrypt(encodedPayload, pass, apiVersion);

			Assert.AreEqual(clearText, decryptedText);
		}
	}
}
