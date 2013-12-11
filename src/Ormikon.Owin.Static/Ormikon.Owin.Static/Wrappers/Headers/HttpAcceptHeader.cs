using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal class HttpAcceptHeader : HttpPropertyHeader
    {
        private const string SplitString = ",";
        private static readonly char[] splitChar = new [] { ',' };

        public HttpAcceptHeader(IDictionary<string, string[]> headers, string code)
            : base(headers, code)
        {
        }

        protected HttpAcceptHeaderValue[] GetAcceptValues()
        {
            return GetEnumValues().Select(v => new HttpAcceptHeaderValue(v)).ToArray();
        }

        protected void SetAcceptValues(ICollection<HttpAcceptHeaderValue> values)
        {
            SetEnumValues(values == null ? null : values.Select(av => av.ToString()).ToList());
        }

        public void AddAcceptValue(string acceptValue)
        {
            AddAcceptValue(new HttpAcceptHeaderValue(acceptValue));
        }

        public void AddAcceptValue(string acceptValue, float qualityFactor)
        {
            AddAcceptValue(new HttpAcceptHeaderValue(acceptValue, qualityFactor));
        }

        public void AddAcceptValue(HttpAcceptHeaderValue acceptValue)
        {
            var values = new List<HttpAcceptHeaderValue>(GetAcceptValues());
            values.Add(acceptValue);
            SetSingleValue(string.Join(SplitString, values.Select(v => v.ToString())));
        }

        public HttpAcceptHeaderValue[] AcceptValues
        {
            get { return GetAcceptValues(); }
        }
    }

    internal class HttpAcceptHeaderValue : HttpPropertyHeaderValue
    {
        private const string QualityFactorProp = "q";

        public HttpAcceptHeaderValue(string acceptValue)
            : base (acceptValue)
        {
        }

        public HttpAcceptHeaderValue(string acceptValue, float qualityFactor)
            : this(acceptValue)
        {
            QualityFactor = qualityFactor;
        }

        public float QualityFactor
        {
            get
            {
                string strValue = this[QualityFactorProp];
                if (string.IsNullOrEmpty(strValue))
                    return 1;
                float result;
                if (float.TryParse(strValue, out result))
                    return result;
                return 1;
            }
            set
            {
                this[QualityFactorProp] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}

