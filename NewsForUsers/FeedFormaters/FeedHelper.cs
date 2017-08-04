using NewsForUsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

namespace NewsForUsers.FeedFormaters
{
    public static class FeedHelper
    {
        public static IEnumerable<Entity> GetEntitiesFromFeed(string urlFeedLocation)
        {
            if (String.IsNullOrEmpty(urlFeedLocation))
                return null;

            using (XmlReader reader = XmlReader.Create(urlFeedLocation))
            {
                Atom10FeedFormatter atom = new Atom10FeedFormatter();
                // try to read it as an atom feed
                if (atom.CanRead(reader))
                {
                    atom.ReadFrom(reader);
                    AtomFormatter formatter = new AtomFormatter();
                    return formatter.GetEntities(urlFeedLocation);
                }

                Rss20FeedFormatter rss = new Rss20FeedFormatter();
                // try reading it as an rss feed
                if (rss.CanRead(reader))
                {
                    rss.ReadFrom(reader);
                    RSSFormater formatter = new RSSFormater();
                    return formatter.GetEntities(urlFeedLocation);
                }

                //add new custom formatters 
            }
            return null;
        }
    }
}