﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Internal;

namespace Grabacr07.KanColleWrapper.Models
{
	internal class SvData<T> : RawDataWrapper<svdata<T>>
	{
		public bool IsSuccess
		{
			get { return this.RawData.api_result == 1; }
		}

		public T Data
		{
			get { return this.RawData.api_data; }
		}

		public kcsapi_deck[] Fleets
		{
			get { return this.RawData.api_data_deck; }
		}

        public NameValueCollection RequestBody { get; set; }


        public SvData(svdata<T> rawData, string reqBody)
            : base(rawData)
        {
            this.RequestBody = HttpUtility.ParseQueryString(reqBody);
        }

	}

	internal class SvData : RawDataWrapper<svdata>
	{
		public bool IsSuccess
		{
			get { return this.RawData.api_result == 1; }
		}

        public NameValueCollection RequestBody { get; set; }


        public SvData(svdata rawData, string reqBody)
            : base(rawData)
        {
            this.RequestBody = HttpUtility.ParseQueryString(reqBody);
        }



		public static SvData<T> Parse<T>(Session session)
		{
			var bytes = Encoding.UTF8.GetBytes(session.GetResponseAsJson());
			var serializer = new DataContractJsonSerializer(typeof(svdata<T>));
			using (var stream = new MemoryStream(bytes))
			{
				var rawResult = serializer.ReadObject(stream) as svdata<T>;
                var result = new SvData<T>(rawResult, session.GetRequestBodyAsString());
                return result;
			}
		}

		public static SvData Parse(Session session)
		{
			var bytes = Encoding.UTF8.GetBytes(session.GetResponseAsJson());
			var serializer = new DataContractJsonSerializer(typeof(svdata));
			using (var stream = new MemoryStream(bytes))
			{
				var rawResult = serializer.ReadObject(stream) as svdata;
                var result = new SvData(rawResult, session.GetRequestBodyAsString());
				return result;
			}
		}

		public static bool TryParse<T>(Session session, out SvData<T> result)
		{
			try
			{
				result = Parse<T>(session);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				result = null;
				return false;
			}

			return true;
		}

		public static bool TryParse(Session session, out SvData result)
		{
			try
			{
				result = Parse(session);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				result = null;
				return false;
			}

			return true;
		}
	}
}
