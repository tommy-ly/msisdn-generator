using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using ValidMsisdnFileGenerate.Web.Models;
using ValidMsisdnFileGenerate.Web.Services;

namespace ValidMsisdnFileGenerate.Web.Controllers
{
    public class HomeController : Controller
    {
        private Faker _faker = new Faker();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateMsisdn(GenerateMsisdnModel model)
        {
            if (model.MsisdnCount > 1000000)
                return View("Index", new HomeViewModel { ErrorMessage = "You cannot generate more than 1000000 msisdns because it will rinse my Azure credits" });

            if (model.MsisdnCount <= 0)
            {
                return View("Index");
            }

            var msisdnGenerator = new MsisdnGenerator();
            var msisdns = msisdnGenerator.GenerateValidMsisdnCollection(model.MsisdnCount, model.PrefixPlus, model.IncludeBadMsisdn);
            var additionalMsisdns = GetAdditionalMsisdn(model.AdditionalMsisdns, model.PrefixPlus);

            msisdns.AddRange(additionalMsisdns);

            byte[] fileContents;

            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                writer.WriteLine(model.RandomData ? 
                    "PhoneNumber,FirstName,LastName,Address1,Job" : 
                    "PhoneNumber");

                foreach (var msisdn in msisdns)
                {
                    var msisdnRow = GenerateMsisdnRow(msisdn, model.RandomData);
                    writer.WriteLine(msisdnRow);
                }

                writer.Flush();
                fileContents = ms.ToArray();
            }

            return File(fileContents, "text/csv", $"{model.Filename ?? model.MsisdnCount.ToString()}.csv");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<string> GetAdditionalMsisdn(string additionalMsisdns, bool prefixPlus)
        {
            if (string.IsNullOrWhiteSpace(additionalMsisdns))
                return new List<string>();

            var msisdns =  additionalMsisdns.Split(new[] {",", "\r\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (prefixPlus)
            {
                for(var i = 0; i < msisdns.Count; i++)
                {
                    if (msisdns[i].StartsWith("+"))
                        continue;

                    msisdns[i] = "+" + msisdns[i];
                }
            }

            return msisdns;
        }

        private string GenerateMsisdnRow(string msisdn, bool includeRandomData)
        {
            var fakeData = msisdn;

            if (includeRandomData)
            {
                fakeData =
                    $"{msisdn},{_faker.Name.FirstName()},{_faker.Name.LastName()},{_faker.Address.StreetAddress()},{_faker.Name.JobTitle()}";
            }

            return fakeData;
        }
    }
}

