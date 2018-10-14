using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EmailDomainExtract
{
    class Program
    {
        internal class domainx
        {
            public string extension { get; set; }
            public string domain { get; set; }
        }
        static void Main(string[] args)
        {
            string inputFilePath = string.Empty;

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.Trim().Length > 1)
                    {
                        if (arg.Trim().Length > 3)
                        {
                            if (arg.Substring(0, 3) == "/i:")
                            {
                                inputFilePath = arg.Substring(3).Trim().ToLower();
                            }
                        }
                    }
                }
            }
            else
            {
                DisplayHelp();
                return;
            }

            if (inputFilePath.Length == 0)
            {
                System.Console.WriteLine("Missing required parameters");
                System.Console.WriteLine("");
                DisplayHelp();
                return;
            }
            else
            {
                var fi = new FileInfo(inputFilePath);
                var di = fi.Directory;
                var domains = new List<domainx>();
                var emailAddresses = new List<string>();
                int incomingCount = 0;
                int incomingDuplicates = 0;

                using (StreamReader sr = new StreamReader(inputFilePath))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        incomingCount++;

                        var emailAddress = line.Trim();
                        if (emailAddresses.Contains(emailAddress) == false)
                        {
                            emailAddresses.Add(emailAddress);
                        }
                        else
                        {
                            incomingDuplicates++;
                        }

                        int index = line.LastIndexOf("@");
                        if (index > -1)
                        {
                            string domain = line.Substring(index + 1).ToLower();
                            domainx x = new domainx()
                            {
                                domain = domain
                            };

                            int extIndex = domain.LastIndexOf(".");
                            if (extIndex > -1)
                            {
                                x.extension = domain.Substring(extIndex + 1).ToLower();

                                bool containsResult = domains.Exists(y => y.domain == domain);
                                if (containsResult == false)
                                {
                                    bool exclude = false;
                                    foreach (var excludeDomain in GetExcludeDomains())
                                    {
                                        if (excludeDomain == x.domain)
                                        {
                                            exclude = true;
                                            break;
                                        }
                                    }

                                    if (exclude == false)
                                    {
                                        domains.Add(x);
                                    }
                                }
                            }
                        }
                        line = sr.ReadLine();
                    }
                }

                domains.Sort(delegate (domainx t1, domainx t2)
                {
                    var compareResult = t1.extension.CompareTo(t2.extension);
                    if (compareResult != 0)
                    {
                        return compareResult;
                    }
                    else
                    {
                        return (t1.domain.CompareTo(t2.domain));
                    }
                });

                emailAddresses.Sort();

                var outputDirectoryPath = Path.Combine(di.FullName, "output");
                if (Directory.Exists(outputDirectoryPath) == true)
                {
                    Directory.Delete(outputDirectoryPath, true);
                }
                Directory.CreateDirectory(outputDirectoryPath);

                foreach (var domain in domains)
                {
                    var outputFilePath = Path.Combine(outputDirectoryPath, domain.extension + "-" + "output-domains.txt");
                    using (StreamWriter sw = new StreamWriter(outputFilePath, true))
                    {
                        sw.WriteLine(domain.domain);
                    }

                    var outputFilePathAll = Path.Combine(outputDirectoryPath, "allextensions-" + "output-domains.txt");
                    using (StreamWriter sw = new StreamWriter(outputFilePathAll, true))
                    {
                        sw.WriteLine(domain.domain);
                    }
                }

                foreach (var emailAddress in emailAddresses)
                {
                    var outputFilePathAll = Path.Combine(outputDirectoryPath, "all-sorted-output-email-addresses.txt");
                    using (StreamWriter sw = new StreamWriter(outputFilePathAll, true))
                    {
                        sw.WriteLine(emailAddress);
                    }
                }

                Console.WriteLine("Incoming Email Addresses=" + incomingCount.ToString());
                Console.WriteLine("Duplicate Email Addresses=" + incomingDuplicates.ToString());

                Console.ReadLine();
            }
        }

        private static void DisplayHelp()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Command Line Arguments:");
            System.Console.WriteLine("/i:xxxxxxx - Input File Path directory");
            System.Console.WriteLine("");
            System.Console.WriteLine("/? = Help");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("Sample command line:");
            System.Console.WriteLine("/b:C:\\TestFiles\\input.txt");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("Output file (Output-Domains.txt) is created is same directory as incoming file");
        }

        private static List<string> GetExcludeDomains()
        {
            var result = new List<string>();

            result.Add("aol.com");
            result.Add("att.net");
            result.Add("gmail.com");
            result.Add("gmx.com");
            result.Add("hotmail.com");
            result.Add("icloud.com");
            result.Add("live.com");
            result.Add("mail.com");
            result.Add("microsoft.com");
            result.Add("outlook.com");
            result.Add("prontomail.com");
            result.Add("yahoo.com");
            result.Add("yandex.com");
            result.Add("zoho.com");

            return result;
        }
    }
}

