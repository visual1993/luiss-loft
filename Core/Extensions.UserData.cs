using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.ComponentModel;
using Visual1993;

namespace Visual1993.Extensions
{
	public class UserData
	{
		/*
		public UserData()
		{
		}
		[JsonIgnore]
		public string NameString { get { return name + " " + surname; } }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string name { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string surname { get; set; }
		public SexType sex { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string mail { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public PasswordHashTypeEnum PasswordHashType = PasswordHashTypeEnum.MD5; //default for backward compatibility 

		//[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		[JsonProperty("md5Password")]
		[Description("Sensible information")]
		public string HashedPassword { get; private set; }
		[JsonIgnore]
		public string PlainTextPassword { 
			set {
				if (PasswordHashType == PasswordHashTypeEnum.MD5) { 
					HashedPassword = TextHelper.CalculateGenericHash(value, MD5.Create());
					return;
				}
				if (PasswordHashType == PasswordHashTypeEnum.Plain)
				{
					HashedPassword = value;
					return;
				}
				if (PasswordHashType == PasswordHashTypeEnum.SHA256)
				{
					HashedPassword = TextHelper.CalculateGenericHash(value, SHA256.Create());
					return;
				}
				HashedPassword = TextHelper.CalculateGenericHash(value);
				return;
			}
		}

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime bornDate { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string accessToken { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string notificationUri { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string mobilePhone { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string fixedPhone { get; set; }
		public SocialProvider provider { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string PlatformSpecificNotificationToken { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool OverwriteMailWithProviderIDAndSuffix { get; set; } = true;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string providerID { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		[Description("Sensible information")]
		public string SocialLastAccessToken { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public CloudImage thumb { get; set; }
		#region legalInfo
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		/// <summary>Codice fiscale</summary>
		public string personID { get; set; } //ITA: codice fiscale
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Residence residence { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public BillingAddress billingAddress { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public HouseAddress houseAddress { get; set; } //ITA: domicilio
		#endregion
		*/
		public enum SexType
		{
			Male=1,
			Female=2,
			Undefined=3
		}
		public enum PasswordHashTypeEnum
		{
			Plain=0,
			MD5 = 1,
			SHA256 = 2,
		}
		public class SocialNetworkRESTResponse
		{
			public string SocialAccessToken { get; set; }
			public string SocialID { get; set; }
			public string UserName { get; set; }
			public string UserMail { get; set; }
			public string UserThumbURL { get; set; }
			public UserData.SocialProvider SocialProvider { get; set; } = SocialProvider.None;
		}
		public enum SocialProvider
		{
			None = 0,
			Facebook = 1,
			Instagram = 2,
			Linkedin = 3,
			Google = 4,
		}
		/*
		public static Dictionary<SocialProvider, string> SocialProviderURIs = new Dictionary<SocialProvider, string> { 
			{SocialProvider.Facebook,Constants.RestAPIV1+"fbLogin.php"},
			{SocialProvider.Linkedin,Constants.RestAPIV1+"linkedinLogin.php"},
			{SocialProvider.Instagram,Constants.RestAPIV1+"instagramLogin.php"},
			{SocialProvider.Google,Constants.RestAPIV1+"googleLogin.php"},
		};
		*/
		public class Residence
		{
			public string street { get; set; }
			public string city { get; set; }
			public string province { get; set; }
			public Nation nation { get; set; }
		}
		public class Nation
		{
			public string label { get; set; }
			public string ISOinfo { get; set; }
			public override string ToString()
			{
				return string.Format("{0}", label);
			}
		}

		public class BillingAddress: Residence
		{
			public bool useSameAsResidence { get; set; }
			public string VAT_ID { get; set; }
		}
		public class HouseAddress : Residence
		{
			public bool useSameAsResidence { get; set; }
		}
	}
}

