namespace DataAccess.Configuration
{
    /*************************************
     * from castle project
     *************************************/

    using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Xml;

    public class XmlConfigurationSource:InPlaceConfigurationSource
    {
        protected XmlConfigurationSource()
		{
		}

        public XmlConfigurationSource(String xmlFileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFileName);
			PopulateSource(doc.DocumentElement);
		}

        public XmlConfigurationSource(Stream stream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            PopulateSource(doc.DocumentElement);
        }

        public XmlConfigurationSource(TextReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            PopulateSource(doc.DocumentElement);
        }


        protected void PopulateSource(XmlNode section)
        {
            PopulateConfigNodes(section);
        }

        private void PopulateConfigNodes(XmlNode section)
        {
            const string Config_Node_Name = "config";

            foreach (XmlNode node in section.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;

                if (!Config_Node_Name.Equals(node.Name))
                {
                    String message = String.Format("Unexpected node. Expect '{0}' found '{1}'",
                                                   Config_Node_Name, node.Name);

                    throw new ConfigurationErrorsException(message);
                }

                String typeName="Default";

                if (node.Attributes.Count != 0)
                {
                    XmlAttribute typeNameAtt = node.Attributes["type"];

                    if (typeNameAtt == null)
                    {
                        String message = String.Format("Invalid attribute at node '{0}'. " +
                                                       "The only supported attribute is 'type'", Config_Node_Name);

                        throw new ConfigurationErrorsException(message);
                    }

                    typeName = typeNameAtt.Value;
                }

                Add(typeName, BuildProperties(node));
            }
        }

        /// <summary>
        /// Builds the configuration properties.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected IDictionary<string, string> BuildProperties(XmlNode node)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (XmlNode addNode in node.SelectNodes("add"))
            {
                XmlAttribute keyAtt = addNode.Attributes["key"];
                XmlAttribute valueAtt = addNode.Attributes["value"];

                if (keyAtt == null || valueAtt == null)
                {
                    String message = String.Format("For each 'add' element you must specify 'key' and 'value' attributes");

                    throw new ConfigurationErrorsException(message);
                }
                string value = valueAtt.Value;

                dict.Add(keyAtt.Value, value);
            }

            return dict;
        }
    }
}
