using System;
using System.Collections.Generic;
using PhoneNumbers;

namespace ValidMsisdnFileGenerate.Web.Services
{
    public class MsisdnGenerator
    {
        Random random = new Random();

        public List<string> GenerateValidMsisdnCollection(int numberOfMsisdns, bool prefixPlus = true, bool includeBadMsisdn = true)
        {
            var msisdns = new List<string>();

            while (msisdns.Count < numberOfMsisdns)
            {
                var msisdn = "447" + random.Next(100000000, 999999999);

                if (prefixPlus)
                {
                    msisdn = "+" + msisdn;
                }

                if (includeBadMsisdn)
                {
                    msisdns.Add(msisdn);
                    continue;
                }

                if (Validate(msisdn))
                {
                    msisdns.Add(msisdn);
                }
            }

            return msisdns;

        }
        private bool Validate(string msisdn)
        {
            var libPhoneNumber = PhoneNumberUtil.GetInstance();
            var phoneNumber = libPhoneNumber.ParseAndKeepRawInput(msisdn, null);

            return libPhoneNumber.IsValidNumber(phoneNumber)
                          && phoneNumber.CountryCodeSource ==
                          PhoneNumber.Types.CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN;
        }
    }
}
