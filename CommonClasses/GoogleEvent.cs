﻿using System;
using Newtonsoft.Json;
using Visual1993.Data;
using System.Collections.Generic;

namespace CommonClasses
{
	public class GoogleEvent
	{
		public GoogleEvent()
		{
		}
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ID { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Description { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime StartDate { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime EndDate { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<string> ImageUris { get; set; } = new List<string>();

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime LastChange { get; set; }


		public class EventsRequest
		{ 
		}
		public class EventsResponse: WebServiceV2.DefaultResponse
		{
			public List<GoogleEvent> items = new List<GoogleEvent>();
		}
		public class RichDescription {

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public List<string> ImageUris { get; set;} = new List<string>();

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Description { get; set; } = "";
		}
	}
}