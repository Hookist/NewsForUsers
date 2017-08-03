﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

namespace NewsForUsers.FeedFormaters
{
    public static class FeedHelper
    {
        public static SyndicationFeed GetSyndicationFeedData(string urlFeedLocation)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                CheckCharacters = true,
                CloseInput = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
            };
            if (String.IsNullOrEmpty(urlFeedLocation))
                return null;

            using (XmlReader reader = XmlReader.Create(urlFeedLocation))
            {
                if (reader.ReadState == ReadState.Initial)
                    reader.MoveToContent();

                Atom10FeedFormatter atom = new Atom10FeedFormatter();
                // try to read it as an atom feed
                if (atom.CanRead(reader))
                {
                    atom.ReadFrom(reader);
                    return atom.Feed;
                }

                Rss20FeedFormatter rss = new Rss20FeedFormatter();
                // try reading it as an rss feed
                if (rss.CanRead(reader))
                {
                    rss.ReadFrom(reader);
                    return rss.Feed;
                }
                // neither?
                return null;
            }
        }
    }
}