using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public class SiteResponse
    {
        Site _site;
        HttpWebResponse response;

        int statusCode;
        string statusCodeText;

        public SiteResponse(Site site)
        {
            _site = site;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_site.Address);
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                SetStatusCodeVariables();
            }
            catch (WebException e)
            {
                WebExceptionStatus status = e.Status;
                switch (status)
                {
                    case WebExceptionStatus.ProtocolError:
                        HttpWebResponse exceptionResponse = (HttpWebResponse) e.Response;
                        statusCode = (int)exceptionResponse.StatusCode;
                        statusCodeText = exceptionResponse.StatusCode.ToString();
                        break;
                }
            }

        }

        public Site GetRespondingSite()
        {
            return _site;
        }

        private void SetStatusCodeVariables()
        {
            statusCode = (int)response.StatusCode;
            statusCodeText = response.StatusCode.ToString();
        }

        public int GetStatusCode()
        {
            return statusCode;
        }

        public string GetStatusCodeText()
        {
            return statusCodeText;
        }

        public int Compare(SiteResponse r1, SiteResponse r2)
        {
            if (r1 == null && r2 == null)
            {
                return 0;
            }
            if (r1 != null && r2 == null)
            {
                return 1;
            }
            if (r1 == null && r2 != null)
            {
                return -1;
            }
            //на этом этапе оба объекта гарантированно != null
            //сравниваются только опрашиваемые сайты
            Site s1 = r1.GetRespondingSite();
            Site s2 = r2.GetRespondingSite();
            if (s1 == null)
            {
                if (s2 == null)
                {
                    return 0;
                }
                else
                {
                    return s2.Compare(s1, s2);
                }
            }
            else
            {
                return s1.Compare(s1, s2);
            }
        }

        public int CompareTo(SiteResponse r)
        {
            if (r == null)
            {
                return -1;
            }
            else
            {
                Site s2 = r.GetRespondingSite();
                Site s1 = this.GetRespondingSite();
                return s1.CompareTo(s2);
            }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            SiteResponse r = obj as SiteResponse;
            if (r as SiteResponse == null)
            {
                return false;
            }
            return this.GetRespondingSite().Equals(r.GetRespondingSite());
        }

        public bool Equals(SiteResponse r)
        {
            if (r == null)
            {
                return false;
            }
            return this.GetRespondingSite().Equals(r.GetRespondingSite());
        }

        public override int GetHashCode()
        {

            int GetNumberFromText(string processingString)
            {
                int number = 0;
                foreach (char symbol in processingString)
                {
                    number += (int)symbol;
                }
                return number;
            }

            int hash = 19;

            hash *= this.GetRespondingSite().GetHashCode();
            //при добавлении полей - дописать сюда их хеширование

            return hash;
        }
    }
}
