using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Host.Controllers
{
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;

    using LinqToLdap;

    class QueueManager
    {
        private static QueueManager queueManager;
        private QueueManager()
        {

        }

        public static QueueManager GetInstance()
        {
            if (queueManager == null)
            {
                queueManager = new QueueManager();
            }

            return queueManager;
        }

        public bool executing = false;
        private readonly ConcurrentQueue<LdapQueueItem> ldapQueue = new ConcurrentQueue<LdapQueueItem>();

        public void AddImageRequest(LdapQueueItem ldapQueueItem)
        {
            Debug.WriteLine(ldapQueue.Count);
            ldapQueue.Enqueue(ldapQueueItem);
            Debug.WriteLine(ldapQueue.Count);
        }

        public bool Contains(LdapQueueItem ldapQueueItem)
        {
            return ldapQueue.Contains(ldapQueueItem);
        }

        public void ProcessImage(HttpContextBase httpContext)
        {
            Debug.WriteLine(ldapQueue.Count);
            if (!executing && ldapQueue.Count > 0)
            {
                executing = true;

                LdapQueueItem ldapQueueItem = null;
                ldapQueue.TryPeek(out ldapQueueItem);
                byte[] image;

                string imageHardDisk = httpContext.Server.MapPath(@"~/Content/LDAP/" + ldapQueueItem.name + ".jpg");
                if (!System.IO.File.Exists(imageHardDisk))
                {
                    image = this.GetActiveDirectoryBinaryImage(ldapQueueItem.name);
                    if (image == null)
                    {
                        image = ImageToByte2(ContentResources.unknown);

                        // image = System.IO.File.ReadAllBytes(httpContext.Server.MapPath(@"Content/LDAP/unknown.jpg"));
                    }

                    FileInfo fileInfo = new FileInfo(imageHardDisk);
                    if (!Directory.Exists(fileInfo.DirectoryName))
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }

                    System.IO.File.WriteAllBytes(imageHardDisk, image);
                }

                ldapQueue.TryDequeue(out ldapQueueItem);
                executing = false;
            }
        }

        private static byte[] ImageToByte2(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        private byte[] GetActiveDirectoryBinaryImage(string assignedTo)
        {
            LdapConnectionFactory factory =
                new LdapConnectionFactory(
                    ConfigurationManager.AppSettings["LdapServerName"] + ":"
                    + ConfigurationManager.AppSettings["LdapServerPort"]);
            using (DirectoryContext context = new DirectoryContext(factory.GetConnection(), disposeOfConnection: true))
            {
                var userMapping = new
                {
                    DistinguishedName = string.Empty,
                    Cn = string.Empty,
                    givenName = string.Empty,
                    ObjectGuid = default(Guid),
                    Members = default(string[]),
                    jpegPhoto = default(byte[]),
                    thumbnailPhoto = default(byte[])
                };

                List<string> words = assignedTo.Split(' ').ToList();
                List<string> wordsToRemove = new List<string>();

                foreach (string word in words)
                {
                    if (word.Length == 1)
                    {
                        wordsToRemove.Add(word);
                    }
                    else
                    {
                        if (word.Last() == '.')
                        {
                            wordsToRemove.Add(word);
                        }
                    }
                }

                words = words.Except(wordsToRemove).Distinct().ToList();

                List<dynamic> users = new List<dynamic>();

                foreach (string word in words)
                {
                    var user =
                    context.Query(userMapping, ConfigurationManager.AppSettings["LdapFilter"], objectClass: "User")
                        .Where(r => r.Cn.Contains(word) && (r.Cn.StartsWith(word) || r.Cn.EndsWith(word)) && r.thumbnailPhoto != null)
                        .FirstOrDefault();

                    if (user != null)
                    {
                        users.Add(user);

                        if (users.Count == 2 && users[0].ObjectGuid == users[1].ObjectGuid)
                        {
                            users.RemoveAt(1);
                            break;
                        }
                    }
                }

                if (users.Count == 1)
                {
                    return users[0].thumbnailPhoto;
                }

                // Not unique object guids - try some comparisons of results (last resort)
                dynamic unknownUser = users.FirstOrDefault(x => words.All(y => x.DistinguishedName.Contains(y)));

                return unknownUser != null ? unknownUser.thumbnailPhoto : null;
            }
        }
    }

    class LdapQueueItem
    {
        public string name;

        public string id;
    }
}