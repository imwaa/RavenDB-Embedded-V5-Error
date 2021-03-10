using Raven.Client.Documents.Session;
using Raven.Embedded;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Xunit;
using Xunit.Abstractions;

namespace Testing_RavenDB_v4._1._6
{
    public class UnitTest
    {
        /*---------------------------------------------------*/
        /*        We got the issue   at line 150             */
        /*---------------------------------------------------*/

        [Fact]
        public void Test1()
        {
            ServerOptions serverOptions = new ServerOptions() { DataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\RavenDB\\Home_import" };
            EmbeddedServer.Instance.StartServer(serverOptions);

            EmbeddedServer.Instance.OpenStudioInBrowser();

            ConvertSocioDemographics();
        }

        private static bool Exists(string filename)
        {
            var store = EmbeddedServer.Instance.GetDocumentStore("Home_import");

            using (IDocumentSession session = store.OpenSession())
            {
                return session.Query<ExternalFileState>().Where(x => x.Filename == Path.GetFileName(filename)).Count() > 0;
            }
        }
        [Fact]
        public static void ConvertSocioDemographics()
        {
            Console.WriteLine("Processing Socio Demographic data");

            // Gets xml files from test data to process them
            string path = @"testdata\xml_GfK\*.socioDemos.a.xml";
            string directory = Path.GetDirectoryName(path);

            string[] files = Directory.GetFiles(directory, Path.GetFileName(path));

            foreach (string file in files)
            {
                if (Exists(Path.GetFileName(file)))
                    continue;


                IList<Home> Homes = new List<Home>();


                using (StreamWriter writer = new StreamWriter(Path.Combine(directory, Path.GetFileName(file) + ".json"), false))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);

                    DateTime vwDay = Convert.ToDateTime(doc.DocumentElement.Attributes["vwDay"].Value);

                    string[] homeQuestions = null;
                    string[] memberQuestions = null;

                    // For each home
                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        // Parse SocioDemos/Specification nodes of the XML (see above)
                        if (node.Name == "Specification")
                        {
                            foreach (XmlNode questions in node.ChildNodes)
                            {
                                if (questions.Name == "Home")
                                {
                                    homeQuestions = new string[Convert.ToInt32(questions.Attributes["nQuestions"].Value)];
                                    string[] responses = questions.Attributes["questionIDs"].Value.Split(' ');
                                    for (int index = 0; index < responses.Length; index++)
                                    {
                                        homeQuestions[index] = responses[index];
                                    }
                                }
                                else
                                {
                                    memberQuestions = new string[Convert.ToInt32(questions.Attributes["nQuestions"].Value)];
                                    string[] responses = questions.Attributes["questionIDs"].Value.Split(' ');
                                    for (int index = 0; index < responses.Length; index++)
                                    {
                                        memberQuestions[index] = responses[index];
                                    }
                                }
                            }
                        }
                        // Parse Homes nodes
                        else
                        {
                            // Process home related data
                            foreach (XmlNode home in node.ChildNodes)
                            {
                                string homeId = home.Attributes["homeID"].Value;
                                string[] homeAnswers = home.Attributes["answerIDs"].Value.Split(' ');

                                SocioDemographicData homeData = new SocioDemographicData();
                                homeData.Answers = new Dictionary<string, string>();
                                for (int index = 0; index < homeAnswers.Length; index++)
                                    homeData.Answers.TryAdd(homeQuestions[index], homeAnswers[index]);

                                Home newHome = new Home() { Id = homeId };
                                newHome.Data.TryAdd(vwDay, homeData);

                                Homes.Add(newHome);

                                // Process member related data
                                foreach (XmlNode member in home.ChildNodes)
                                {
                                    string memberID = member.Attributes["memberID"].Value;
                                    double weight = Convert.ToDouble(member.Attributes["weight"].Value.Replace(".", ","));
                                    string[] memberAnswers = member.Attributes["answerIDs"].Value.Split(' ');

                                    SocioDemographicData memberData = new SocioDemographicData();
                                    memberData.Weight = weight;
                                    memberData.Answers = new Dictionary<string, string>();
                                    for (int index = 0; index < memberAnswers.Length; index++)
                                        memberData.Answers.TryAdd(memberQuestions[index], memberAnswers[index]);

                                    Member newMember = new Member() { Id = memberID };
                                    newMember.HomeId = newHome.Id;
                                    newMember.Data.TryAdd(vwDay, memberData);

                                    newHome.Members.Add(newMember);

                                }
                            }
                        }
                    }
                }

                var store = EmbeddedServer.Instance.GetDocumentStore("Home_import");

                foreach (var home in Homes)
                {
                    using (IDocumentSession session = store.OpenSession())
                    {
                        /*---------------------------------------------------*/
                        /*        Here is where we get the issue             */
                        /*---------------------------------------------------*/
                        var existingHome = session.Load<Home>("home/" + home.Id);
                        if (existingHome == null)
                        {
                            session.Store(home, "home/" + home.Id);
                        }
                        else
                        {
                            // Update home data
                            foreach (var data in home.Data.Keys)
                                if (!existingHome.Data.ContainsKey(data))
                                    existingHome.Data.Add(data, home.Data[data]);

                            // Add member ?
                            foreach (var member in home.Members)
                            {
                                var existingMember = existingHome.Members.Where(x => x.Id == member.Id).SingleOrDefault();

                                if (existingMember == null)
                                {
                                    existingHome.Members.Add(member);
                                }
                                else
                                {
                                    // Update member
                                    foreach (var data in member.Data.Keys)
                                        if (!existingMember.Data.ContainsKey(data))
                                            existingMember.Data.Add(data, member.Data[data]);
                                }
                            }
                        }

                        session.SaveChanges();
                    }
                }


                using (IDocumentSession session = store.OpenSession())
                {
                    session.Store(ExternalFileState.GetFromFilename(file));
                    session.SaveChanges();
                }
            }
        }
    }
}
