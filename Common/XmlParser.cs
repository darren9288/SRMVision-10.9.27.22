using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Common
{
    public class XmlParser
    {
        private bool m_blnNewSection = false;
        private string m_fileFullName = "";
        private XmlDocument m_xmlDocument;
        private XmlDocument m_xmlDocument2;
        private XmlElement m_firstSectionElement;
        private XmlElement m_secondSectionElement;
        private XmlElement m_thirdSectionElement;
        private XmlElement m_fourthSectionElement;



        /// <summary>
        /// Delete the XML file first before rewrite all settings
        /// </summary>
        /// <param name="strfileFullName">path to store XML file</param>
        public XmlParser(string strfileFullName)
        {
            LoadFile(strfileFullName);
        }

        /// <summary>
        /// Delete the XML file first before rewrite all settings
        /// </summary>
        /// <param name="strfileFullName">path to store XML file</param>
        /// <param name="blnNewFile">If yes, delete the file. Default is false</param>
        public XmlParser(string strfileFullName, bool blnNewFile)
        {
            if (blnNewFile)
            {
                if (File.Exists(strfileFullName))
                {
                    File.Delete(strfileFullName);
                }
            }

            LoadFile(strfileFullName);
        }

        /// <summary>
        /// Delete the XML file first before rewrite all settings
        /// </summary>
        /// <param name="strfileFullName">path to store XML file</param>
        public XmlParser(string strfileFullName, string strRootName)
        {
            LoadFile(strfileFullName, strRootName);
        }

        /// <summary>
        /// Clear first level section of xml 
        /// <Root><Books></Books></Root>
        /// </summary>
        public void ClearFirstSection()
        {
            if (m_firstSectionElement != null)
            {
                XmlElement elementNode = m_firstSectionElement;
                elementNode.InnerText = "";
                elementNode.InnerXml = "";
            }
        }

        /// <summary>
        /// Clear second level section of xml 
        /// <Root><Books><book></book></Books></Root>
        /// </summary>
        public void ClearSecondSection()
        {
            if (m_secondSectionElement != null)
            {
                XmlElement elementNode = m_secondSectionElement;
                elementNode.InnerText = "";
                elementNode.InnerXml = "";
            }
        }

        /// <summary>
        /// Clear third level section of xml 
        /// <Root><Books><book><author></author></book></Books></Root>
        /// </summary>
        public void ClearThirdSection()
        {
            if (m_thirdSectionElement != null)
            {
                XmlElement elementNode = m_thirdSectionElement;
                elementNode.InnerText = "";
                elementNode.InnerXml = "";
            }
        }

        /// <summary>
        /// Clear fourth section of xml 
        /// <Root><Books><book><author></author></book></Books></Root>
        /// </summary>
        public void ClearFourthSection()
        {
            if (m_fourthSectionElement != null)
            {
                XmlElement elementNode = m_fourthSectionElement;
                elementNode.InnerText = "";
                elementNode.InnerXml = "";
            }
        }


        /// <summary>
        /// Get First Level Section Content
        /// <Root><Books></Books></Root>
        /// </summary>
        /// <param name="strSection">Section Name = "Books"</param>
        public void GetFirstSection(string strSection)
        {
            strSection = strSection.Replace(" ", string.Empty);

            if (m_xmlDocument.DocumentElement != null)
                m_firstSectionElement = m_xmlDocument.DocumentElement[strSection];
        }

        /// <summary>
        /// Get Secod Level Section Content
        /// <Root><Books><book></book></Books></Root>
        /// </summary>
        /// <param name="strSection">Section Name = "book"</param>
        public void GetSecondSection(string strSection)
        {
            strSection = strSection.Replace(" ", string.Empty);

            if (m_firstSectionElement != null)
                m_secondSectionElement = m_firstSectionElement[strSection];
        }

        /// <summary>
        /// Get Third Level Section Content
        /// <Root><Books><book><author></author></book></Books></Root>
        /// </summary>
        /// <param name="strSection">Section Name = "author"</param>
        public void GetThirdSection(string strSection)
        {
            strSection = strSection.Replace(" ", string.Empty);

            if (m_secondSectionElement != null)
                m_thirdSectionElement = m_secondSectionElement[strSection];
        }

        /// <summary>
        /// Get Fourth Level Section Content
        /// <Root><Books><book><author></author></book></Books></Root>
        /// </summary>
        /// <param name="strSection">Section Name = "author"</param>
        public void GetFourthSection(string strSection)
        {
            strSection = strSection.Replace(" ", string.Empty);

            if (m_thirdSectionElement != null)
                m_fourthSectionElement = m_thirdSectionElement[strSection];
        }



        /// <summary>
        /// Get First Section's Child Nodes' Counter
        /// <Root><Books></Books></Root>
        /// </summary>
        /// <returns>Child Nodes' Count in integer. Example : return 1</returns>
        public int GetFirstSectionCount()
        {
            return m_xmlDocument.DocumentElement.ChildNodes.Count;
        }

        /// <summary>
        /// Get Second Section's Child Nodes' Counter
        /// <Root><Books><book></book><book></book></Books></Root>
        /// </summary>
        /// <returns>Child Nodes' Count in integer. Example : return 2</returns>
        public int GetSecondSectionCount()
        {
            if (m_firstSectionElement != null)
                return m_firstSectionElement.ChildNodes.Count;

            return 0;
        }

        /// <summary>
        /// Get Third Section's Child Nodes' Counter
        /// <Root><Books><book><author></author></book></Books></Root>
        /// </summary>
        /// <returns>Child Nodes' Count in integer. Example : return 1</returns>
        public int GetThirdSectionCount()
        {
            if (m_secondSectionElement != null)
                return m_secondSectionElement.ChildNodes.Count;

            return 0;
        }

        /// <summary>
        /// Get Fourth Section's Child Nodes' Counter
        /// <Root><Books><book><author></author></book></Books></Root>
        /// </summary>
        /// <returns>Child Nodes' Count in integer. Example : return 1</returns>
        public int GetFourthSectionCount()
        {
            if (m_thirdSectionElement != null)
                return m_thirdSectionElement.ChildNodes.Count;

            return 0;
        }

        /// <summary>
        /// Get Fifth Section's Child Nodes' Counter
        /// <Root><Books><book><author></author></book></Books></Root>
        /// </summary>
        /// <returns>Child Nodes' Count in integer. Example : return 1</returns>
        public int GetFifthSectionCount()
        {
            if (m_fourthSectionElement != null)
                return m_fourthSectionElement.ChildNodes.Count;

            return 0;
        }



        /// <summary>
        /// Get particular Second Section Child Node's name
        /// </summary>
        /// <param name="strParentName">parent name</param>
        /// <param name="intCount">child node index</param>
        /// <returns>child node's name in string</returns>
        public string GetSecondSectionElement(string strParentName, int intCount)
        {
            if (m_firstSectionElement != null)
                return m_firstSectionElement.ChildNodes[intCount].Name;
            return "";
        }

        /// <summary>
        /// Get particular Third Section Child Node's name
        /// </summary>
        /// <param name="strParentName">parent name</param>
        /// <param name="intCount">child node index</param>
        /// <returns>child node's name in string</returns>
        public string GetThirdSectionElement(string strParentName, int intCount)
        {
            if (m_secondSectionElement != null)
                return m_secondSectionElement.ChildNodes[intCount].Name;
            return "";
        }

        /// <summary>
        /// Get particular Fourth Section Child Node's name
        /// </summary>
        /// <param name="strParentName">parent name</param>
        /// <param name="intCount">child node index</param>
        /// <returns>child node's name in string</returns>
        public string GetFourthSectionElement(string strParentName, int intCount)
        {
            if (m_thirdSectionElement != null)
                return m_thirdSectionElement.ChildNodes[intCount].Name;
            return "";
        }

        /// <summary>
        /// Get particular section child node's name
        /// </summary>
        /// <param name="intIndex">child node index</param>
        /// <param name="intNodeLevel">node level/section</param>
        /// <returns>child node's name in string</returns>
        public string GetElementName(int intIndex, int intNodeLevel)
        {
            switch (intNodeLevel)
            {
                case 1:
                    if (m_firstSectionElement != null)
                        return m_firstSectionElement.ChildNodes[intIndex].Name;
                    break;
                case 2:
                    if (m_secondSectionElement != null)
                        return m_secondSectionElement.ChildNodes[intIndex].Name;
                    break;
                case 3:
                    if (m_thirdSectionElement != null)
                        return m_firstSectionElement.ChildNodes[intIndex].Name;
                    break;
                case 4:
                    if (m_thirdSectionElement != null)
                        return m_firstSectionElement.ChildNodes[intIndex].Name;
                    break;
            }

            return "";
        }


        /// <summary>
        /// Return node value in string type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public string GetValueAsString(string strElementName, string strDefaultValue)
        {
            return GetValueAsString(strElementName, strDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in string type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in string</returns>
        public string GetValueAsString(string strElementName, string strDefaultValue, int intNodeLevel)
        {
            strElementName = strElementName.Replace(" ", String.Empty);

            switch (intNodeLevel)
            {
                case 1:
                    if (m_firstSectionElement != null)
                    {
                        XmlNode node = m_firstSectionElement.SelectSingleNode(strElementName);
                        if (node != null)
                            return node.InnerText;
                    }
                    break;
                case 2:
                    if (m_secondSectionElement != null)
                    {
                        XmlNode node = m_secondSectionElement.SelectSingleNode(strElementName);
                        if (node != null)
                            return node.InnerText;
                    }
                    break;
                case 3:
                    if (m_thirdSectionElement != null)
                    {
                        XmlNode node = m_thirdSectionElement.SelectSingleNode(strElementName);
                        if (node != null)
                            return node.InnerText;
                    }
                    break;
                case 4:
                    if (m_fourthSectionElement != null)
                    {
                        XmlNode node = m_fourthSectionElement.SelectSingleNode(strElementName);
                        if (node != null)
                            return node.InnerText;
                    }
                    break;
            }

            return strDefaultValue;
        }

        /// <summary>
        /// Return node value in decimal type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public decimal GetValueAsDecimal(string strElementName, decimal dDefaultValue)
        {
            return GetValueAsDecimal(strElementName, dDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in decimalv type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="intDefaultValue">Default value for this node</param>
        /// <param name="intType">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in integer</returns>
        public decimal GetValueAsDecimal(string strElementName, decimal dDefaultValue, int intType)
        {
            return Convert.ToDecimal(GetValueAsString(strElementName, dDefaultValue.ToString(), intType));
        }


        /// <summary>
        /// Return node value in Double type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public double GetValueAsDouble(string strElementName, double dDefaultValue)
        {
            return GetValueAsDouble(strElementName, dDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in Double type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="dDefaultValue">Default value for this node</param>
        /// <param name="intType">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in double</returns>
        public double GetValueAsDouble(string strElementName, double dDefaultValue, int intType)
        {
            return Convert.ToDouble(GetValueAsString(strElementName, dDefaultValue.ToString(), intType));
        }



        /// <summary>
        /// Return node value in integer type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public int GetValueAsInt(string strElementName, int intDefaultValue)
        {
            return GetValueAsInt(strElementName, intDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in integer type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public long GetValueAsLong(string strElementName, int intDefaultValue)
        {
            return GetValueAsLong(strElementName, intDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in integer type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="intDefaultValue">Default value for this node</param>
        /// <param name="intType">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in integer</returns>
        public int GetValueAsInt(string strElementName, int intDefaultValue, int intType)
        {
            int intOutput = intDefaultValue;
            if (int.TryParse(GetValueAsString(strElementName, intDefaultValue.ToString(), intType), out intOutput))
            {
                return intOutput;
            }
            else
                return intDefaultValue;

            //return Convert.ToInt32(GetValueAsString(strElementName, intDefaultValue.ToString(), intType));
        }

        /// <summary>
        /// Return node value in integer type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="intDefaultValue">Default value for this node</param>
        /// <param name="intType">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in integer</returns>
        public long GetValueAsLong(string strElementName, int intDefaultValue, int intType)
        {
            return Convert.ToInt64(GetValueAsString(strElementName, intDefaultValue.ToString(), intType));
        }

        /// <summary>
        /// Return node value in Boolean type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public bool GetValueAsBoolean(string strElementName, bool blnDefaultValue)
        {
            return GetValueAsBoolean(strElementName, blnDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in Boolean type
        /// </summary>
        /// <param name="strElementName"></param>
        /// <param name="blnDefaultValue">Default value for this node</param>
        /// <param name="intType">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in boolean</returns>
        public bool GetValueAsBoolean(string strElementName, bool blnDefaultValue, int intType)
        {
            switch (intType)
            {
                case 1:
                    if (m_firstSectionElement != null)
                    {
                        XmlNode node = m_firstSectionElement.SelectSingleNode(strElementName);
                        if (node != null)
                            return (node.InnerText == "1");
                    }
                    break;
                case 2:
                    if (m_secondSectionElement != null)
                    {
                        XmlNode node = m_secondSectionElement.SelectSingleNode(strElementName);
                        if (node != null)
                            return (node.InnerText == "1");
                    }
                    break;
                case 3:
                    if (m_thirdSectionElement != null)
                    {
                        XmlNode node = m_thirdSectionElement.SelectSingleNode(strElementName);
                        if (node != null)
                            return (node.InnerText == "1");
                    }
                    break;
            }

            return blnDefaultValue;
        }



        /// <summary>
        /// Return node value in float type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public float GetValueAsFloat(string strElementName, float fDefaultValue)
        {
            return GetValueAsFloat(strElementName, fDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in float type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="fDefaultValue">Default value for this node</param>
        /// <param name="intType">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in float</returns>
        public float GetValueAsFloat(string strElementName, float fDefaultValue, int intType)
        {
            float fOutput = fDefaultValue;
            if (float.TryParse(GetValueAsString(strElementName, fDefaultValue.ToString(), intType), out fOutput))
            {
                return fOutput;
            }
            else
                return fDefaultValue;

            //return float.Parse(GetValueAsString(strElementName, fDefaultValue.ToString(), intType));
        }



        /// <summary>
        /// Return node value in uint type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="strDefaultValue">Default value for this node</param>
        /// <param name="intNodeLevel">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        public UInt32 GetValueAsUInt(string strElementName, UInt32 intDefaultValue)
        {
            return GetValueAsUInt(strElementName, intDefaultValue, 1);
        }

        /// <summary>
        /// Return node value in uint type
        /// </summary>
        /// <param name="strElementName">Node element name</param>
        /// <param name="intDefaultValue">Default value for this node</param>
        /// <param name="intType">1 = Node level 1; 2 = Node level 2; Default is 1</param>
        /// <returns>node value in uint</returns>
        public UInt32 GetValueAsUInt(string strElementName, UInt32 intDefaultValue, int intType)
        {
            return Convert.ToUInt32(GetValueAsString(strElementName, intDefaultValue.ToString(), intType));
        }



        /// <summary>
        /// If xml node name is same for more than 2 node, use this function
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="strValue">node value</param>
        public void AppendElement1Value(string strElement, string strValue)
        {
            strElement = strElement.Replace(" ", String.Empty);

            if (m_firstSectionElement != null)
            {
                XmlElement newElement = m_xmlDocument.CreateElement(strElement);
                newElement.InnerText = strValue;
                m_firstSectionElement.AppendChild(newElement);

                m_secondSectionElement = m_firstSectionElement[strElement];
            }
        }

        /// <summary>
        /// If xml node name is same for more than 2 node, use this function
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="strValue">node value</param>
        public void AppendElement2Value(string strElement, string strValue)
        {
            strElement = strElement.Replace(" ", String.Empty);

            if (m_secondSectionElement != null)
            {
                XmlElement newElement = m_xmlDocument.CreateElement(strElement);
                newElement.InnerText = strValue;
                m_secondSectionElement.AppendChild(newElement);

                m_thirdSectionElement = m_secondSectionElement[strElement];
            }
        }

        /// <summary>
        /// If xml node name is same for more than 2 node, use this function
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="strValue">node value</param>
        public void AppendElement3Value(string strElement, string strValue)
        {
            strElement = strElement.Replace(" ", String.Empty);

            if (m_thirdSectionElement != null)
            {
                XmlElement newElement = m_xmlDocument.CreateElement(strElement);
                newElement.InnerText = strValue;
                m_thirdSectionElement.AppendChild(newElement);

                m_fourthSectionElement = m_thirdSectionElement[strElement];
            }
        }

        /// <summary>
        /// If xml node name is same for more than 2 node, use this function
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="strValue">node value</param>
        public void AppendElement4Value(string strElement, string strValue)
        {
            strElement = strElement.Replace(" ", String.Empty);

            if (m_thirdSectionElement != null)
            {
                XmlElement newElement = m_xmlDocument.CreateElement(strElement);
                newElement.InnerText = strValue;
                m_fourthSectionElement.AppendChild(newElement);
            }
        }

        /// <summary>
        /// start and write a new element
        /// </summary>
        /// <param name="section">parent name</param>
        /// <param name="blnClearNodes">if true, if parent name is exist, delete all child nodes insider first. If false, just rewrite the info</param>
        public void WriteSectionElement(string section)
        {
            WriteSectionElement(section, false);
        }
        public void WriteSectionElement(string section, bool blnClearNodes)
        {
            WriteEndSection();

            section = section.Replace(" ", String.Empty);

            if (m_xmlDocument.DocumentElement != null)
            {
                m_firstSectionElement = m_xmlDocument.DocumentElement[section];

                if (m_firstSectionElement == null)
                {
                    m_firstSectionElement = m_xmlDocument.CreateElement(section);
                    m_blnNewSection = true;
                }
                else if (blnClearNodes)
                {
                    m_firstSectionElement.RemoveAll();
                }
            }
        }


        public void WriteElement1Value(string element, bool value)
        {
            WriteElement1Value(element, value, "", true);
        }

        public void WriteElement1Value(string element, float value)
        {
            WriteElement1Value(element, value, "", true);
        }

        public void WriteElement1Value(string element, int value)
        {
            WriteElement1Value(element, value, "", true);
        }

        public void WriteElement1Value(string element, double value)
        {
            WriteElement1Value(element, value, "", true);
        }

        public void WriteElement1Value(string element, string value)
        {
            WriteElement1Value(element, value, "", true);
        }

        public void WriteElement1Value(string element, long value)
        {
            WriteElement1Value(element, value, "", true);
        }

        public void WriteElement2Value(string element, bool value)
        {
            WriteElement2Value(element, value, "", true);
        }

        public void WriteElement2Value(string element, float value)
        {
            WriteElement2Value(element, value, "", true);
        }

        public void WriteElement2Value(string element, int value)
        {
            WriteElement2Value(element, value, "", true);
        }

        public void WriteElement2Value(string element, double value)
        {
            WriteElement2Value(element, value, "", true);
        }

        public void WriteElement2Value(string element, string value)
        {
            WriteElement2Value(element, value, "", true);
        }

        public void WriteElement3Value(string element, bool value)
        {
            WriteElement3Value(element, value, "", true);
        }

        public void WriteElement3Value(string element, float value)
        {
            WriteElement3Value(element, value, "", true);
        }

        public void WriteElement3Value(string element, int value)
        {
            WriteElement3Value(element, value, "", true);
        }

        public void WriteElement3Value(string element, double value)
        {
            WriteElement3Value(element, value, "", true);
        }

        public void WriteElement3Value(string element, string value)
        {
            WriteElement3Value(element, value, "", true);
        }

        public void WriteElement4Value(string element, bool value)
        {
            WriteElement4Value(element, value, "", true);
        }

        public void WriteElement4Value(string element, float value)
        {
            WriteElement4Value(element, value, "", true);
        }

        public void WriteElement4Value(string element, int value)
        {
            WriteElement4Value(element, value, "", true);
        }

        public void WriteElement4Value(string element, double value)
        {
            WriteElement4Value(element, value, "", true);
        }

        public void WriteElement4Value(string element, string value)
        {
            WriteElement4Value(element, value, "", true);
        }



        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="element">node name</param>
        /// <param name="value">node value in boolean</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement1Value(string element, bool value, string strDescription, bool blnOverwrite)
        {
            if (value)
                WriteElement1Value(element, "1", strDescription, blnOverwrite);
            else
                WriteElement1Value(element, "0", strDescription, blnOverwrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="element">node name</param>
        /// <param name="value">node value in float</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement1Value(string element, float value, string strDescription, bool blnOverwrite)
        {
            WriteElement1Value(element, value.ToString(), strDescription, blnOverwrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="element">node name</param>
        /// <param name="value">node value in int</param>
        public void WriteElement1Value(string element, int value, string strDescription, bool blnOverwrite)
        {
            WriteElement1Value(element, value.ToString(), strDescription, true);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="element">node name</param>
        /// <param name="value">node value in double</param>
        /// <param name="blnOverwrite"></param>
        public void WriteElement1Value(string element, double value, string strDescription, bool blnOverwrite)
        {
            WriteElement1Value(element, value.ToString(), strDescription, blnOverwrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="element">node name</param>
        /// <param name="value">node value in float</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement1Value(string element, long value, string strDescription, bool blnOverwrite)
        {
            WriteElement1Value(element, value.ToString(), strDescription, blnOverwrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="element">node name</param>
        /// <param name="value">node value in string</param>
        /// <param name="strDescription">node detail description for displayed title</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement1Value(string element, string value, string strDescription, bool blnOverwrite)
        {
            element = element.Replace(" ", string.Empty);

            if (m_firstSectionElement != null)
            {
                XmlElement elementNode = m_firstSectionElement[element];
                // Write if xml is blank
                if (elementNode == null || !blnOverwrite)
                {
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;

                    XmlElement newElement = m_xmlDocument.CreateElement(element);
                    newElement.SetAttributeNode(descAttribute);
                    newElement.InnerText = value;
                    m_firstSectionElement.AppendChild(newElement);
                }
                // if value not "", mean asking to add.
                // if value is "", mean asking to clear child xml
                // if blnSkip is true, mean don't modify child xml
                else if (blnOverwrite)
                {
                    elementNode.InnerText = value;
                    //2020-12-29 ZJYEOH : save description for edit lot 
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;
                    elementNode.SetAttributeNode(descAttribute);
                }
                m_secondSectionElement = m_firstSectionElement[element];
            }
        }

        //-------------------------------------write at the root of the XML file------------------------------------
        //---------------------------------currently only for write SECSGEM.xml use---------------------------------
        //-------------------------------if no value will write "NA" into the XML file------------------------------

        public void WriteRootElement(string element)
        {
            if (m_xmlDocument.DocumentElement == null)
                m_xmlDocument.AppendChild(m_xmlDocument.CreateElement(element));
        }

        public void WriteElementValue(string element, string value)
        {
            element = element.Replace(" ", string.Empty);

            if (m_xmlDocument.DocumentElement != null)
            {
                XmlElement elementNode = m_xmlDocument.DocumentElement[element];

                if (elementNode == null)
                {
                    XmlElement newElement = m_xmlDocument.CreateElement(element);
                    newElement.InnerText = value;
                    m_xmlDocument.DocumentElement.AppendChild(newElement);
                }
                else
                    elementNode.InnerText = value;
            }
        }

        public void WriteElementValue(string element, int value)
        {
            WriteElementValue(element, value.ToString());
        }

        public void WriteElementValue(string element, long value)
        {
            WriteElementValue(element, value.ToString());
        }

        public void WriteElementValue(string element, float value)
        {
            WriteElementValue(element, value.ToString());
        }

        public void WriteElementValue(string element, bool blnValue)
        {
            if (blnValue)
                WriteElementValue(element, "1");
            else
                WriteElementValue(element, "0");
        }

        //----------------------------------------------------------------------------------------------------------


        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="blnValue">node value in boolean</param>     
        /// <param name="strDescription">node detail description for displayed title</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement2Value(string strElement, bool blnValue, string strDescription, bool blnOverwrite)
        {
            if (blnValue)
                WriteElement2Value(strElement, "1", strDescription, blnOverwrite);
            else
                WriteElement2Value(strElement, "0", strDescription, blnOverwrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="intValue">node value in int</param>
        /// <param name="strDescription">node detail description for displayed title</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement2Value(string strElement, int intValue, string strDescription, bool blnOverwrite)
        {
            WriteElement2Value(strElement, intValue.ToString(), strDescription, blnOverwrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="dValue">node value in double</param>
        /// <param name="strDescription">node detail description for displayed title</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement2Value(string strElement, double dValue, string strDescription, bool blnOverwrite)
        {
            WriteElement2Value(strElement, dValue.ToString(), strDescription, blnOverwrite);
        }


        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="strValue">node value in string</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement2Value(string strElement, string strValue, string strDescription, bool blnOverwrite)
        {
            strElement = strElement.Replace(" ", string.Empty);

            if (m_secondSectionElement != null)
            {
                XmlElement elementNode = m_secondSectionElement[strElement];

                if (elementNode == null || !blnOverwrite)
                {
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;

                    XmlElement newElement = m_xmlDocument.CreateElement(strElement);
                    newElement.SetAttributeNode(descAttribute);
                    newElement.InnerText = strValue;
                    m_secondSectionElement.AppendChild(newElement);
                }
                else if (blnOverwrite)
                {
                    elementNode.InnerText = strValue;
                    //2020-12-29 ZJYEOH : save description for edit lot 
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;
                    elementNode.SetAttributeNode(descAttribute);
                }
                m_thirdSectionElement = m_secondSectionElement[strElement];
            }
        }



        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="blnValue">node value in bool</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement3Value(string strElement, bool blnValue, string strDescription, bool blnOverwrite)
        {
            if (blnValue)
                WriteElement3Value(strElement, "1", strDescription, blnOverwrite);
            else
                WriteElement3Value(strElement, "0", strDescription, blnOverwrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="intValue">node value in int</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement3Value(string strElement, int intValue, string strDescription, bool blnOverWrite)
        {
            WriteElement3Value(strElement, intValue.ToString(), strDescription, blnOverWrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="dValue">node value in double</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement3Value(string strElement, double dValue, string strDescription, bool blnOverWrite)
        {
            WriteElement3Value(strElement, dValue.ToString(), strDescription, blnOverWrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="strValue">node value in string</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement3Value(string strElement, string strValue, string strDescription, bool blnOverwrite)
        {
            strElement = strElement.Replace(" ", string.Empty);

            if (m_thirdSectionElement != null)
            {
                XmlElement elementNode = m_thirdSectionElement[strElement];
                if (elementNode == null || !blnOverwrite)
                {
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;

                    XmlElement newElement = m_xmlDocument.CreateElement(strElement);
                    newElement.SetAttributeNode(descAttribute);
                    newElement.InnerText = strValue;
                    m_thirdSectionElement.AppendChild(newElement);
                }
                else if (blnOverwrite)
                {
                    elementNode.InnerText = strValue;
                    //2020-12-29 ZJYEOH : save description for edit lot 
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;
                    elementNode.SetAttributeNode(descAttribute);
                }
                m_fourthSectionElement = m_thirdSectionElement[strElement];
            }
        }



        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="blnValue">node value in bool</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement4Value(string strElement, bool blnValue, string strDescription, bool blnOverWrite)
        {
            if (blnValue)
                WriteElement4Value(strElement, "1", strDescription, blnOverWrite);
            else
                WriteElement4Value(strElement, "0", strDescription, blnOverWrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="intValue">node value in int</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement4Value(string strElement, int intValue, string strDescription, bool blnOverWrite)
        {
            WriteElement4Value(strElement, intValue.ToString(), strDescription, blnOverWrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="dValue">node value in double</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement4Value(string strElement, double dValue, string strDescription, bool blnOverWrite)
        {
            WriteElement4Value(strElement, dValue.ToString(), strDescription, blnOverWrite);
        }

        /// <summary>
        /// if each xml node is using different name, use this function to write
        /// if node is not exist, create a new node with element name
        /// if node is exist, overwrite its value
        /// </summary>
        /// <param name="strElement">node name</param>
        /// <param name="strValue">node value in string</param>
        /// <param name="strDescription">Detail description on node</param>
        /// <param name="blnOverwrite">true = overwrite value if node exist, false = remain value if node exist already</param>
        public void WriteElement4Value(string strElement, string strValue, string strDescription, bool blnOverwrite)
        {
            strElement = strElement.Replace(" ", string.Empty);

            if (m_fourthSectionElement != null || !blnOverwrite)
            {
                XmlElement elementNode = m_fourthSectionElement[strElement];
                if (elementNode == null)
                {
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;

                    XmlElement newElement = m_xmlDocument.CreateElement(strElement);
                    newElement.SetAttributeNode(descAttribute);
                    newElement.InnerText = strValue;
                    m_fourthSectionElement.AppendChild(newElement);
                }
                else if (blnOverwrite)
                {
                    elementNode.InnerText = strValue;
                    //2020-12-29 ZJYEOH : save description for edit lot 
                    XmlAttribute descAttribute = m_xmlDocument.CreateAttribute("description");
                    descAttribute.Value = strDescription;
                    elementNode.SetAttributeNode(descAttribute);
                }
            }
        }


        /// <summary>
        /// Write the end section and save the xml file
        /// </summary>
        public void WriteEndElement()
        {
            WriteEndSection();

            m_xmlDocument.Save(m_fileFullName);
        }



        /// <summary>
        /// Create directory that does not exist
        /// </summary>
        /// <param name="directory">directory</param>
        private void CreateUnexistDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
            {
                CreateUnexistDirectory(directory.Parent);
            }

            Directory.CreateDirectory(directory.FullName);
        }

        /// <summary>
        /// Load specific file, create a new directory if the directory is not found and create xml file if the file is not found
        /// </summary>
        /// <param name="strFileFullName">file name</param>
        private void LoadFile(string strFileFullName)
        {
            try
            {
                m_fileFullName = strFileFullName;
                m_xmlDocument = new XmlDocument();
                m_xmlDocument.Load(m_fileFullName);
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
                FileInfo fi = new FileInfo(strFileFullName);
                if (!fi.Exists)
                {
                    string strDirectoryName = Path.GetDirectoryName(strFileFullName);
                    DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
                    if (!directory.Exists)
                        CreateUnexistDirectory(directory);

                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }

                //if file is not found, create a new xml file
                XmlTextWriter xmlWriter = new XmlTextWriter(strFileFullName, System.Text.Encoding.UTF8);
                xmlWriter.WriteStartElement("Root");
                xmlWriter.Close();
                m_xmlDocument.Load(strFileFullName);
            }
        }

        /// <summary>
        /// Load specific file, create a new directory if the directory is not found and create xml file if the file is not found
        /// </summary>
        /// <param name="strFileFullName">file name</param>
        private void LoadFile(string strFileFullName, string strRootName)
        {
            try
            {
                m_fileFullName = strFileFullName;
                m_xmlDocument = new XmlDocument();
                m_xmlDocument.Load(m_fileFullName);
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
                FileInfo fi = new FileInfo(strFileFullName);
                if (!fi.Exists)
                {
                    string strDirectoryName = Path.GetDirectoryName(strFileFullName);
                    DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
                    if (!directory.Exists)
                        CreateUnexistDirectory(directory);

                    //Create a file to write to.
                    StreamWriter sw = fi.CreateText();
                    sw.Close();
                }

                //if file is not found, create a new xml file
                XmlTextWriter xmlWriter = new XmlTextWriter(strFileFullName, System.Text.Encoding.UTF8);
                xmlWriter.WriteStartElement(strRootName);
                xmlWriter.Close();
                m_xmlDocument.Load(strFileFullName);
            }
        }

        /// <summary>
        /// Write the end section
        /// </summary>
        private void WriteEndSection()
        {
            if (m_blnNewSection && m_firstSectionElement != null)
            {
                m_xmlDocument.DocumentElement.AppendChild(m_firstSectionElement);
                m_blnNewSection = false;
            }
        }

        public void CopyNode(string strPath, string strSectionName)
        {
            m_xmlDocument2 = new XmlDocument();
            m_xmlDocument2.Load(strPath);

            strSectionName = strSectionName.Replace(" ", String.Empty);

            if (m_xmlDocument2.DocumentElement != null)
            {
                XmlElement SectionElement = m_xmlDocument2.DocumentElement[strSectionName];
                if (SectionElement != null)
                {
                    m_firstSectionElement.InnerXml = SectionElement.InnerXml;
                }
            }
        }

        public int GetSectionNodesCount(int intIndex)
        {
            return m_xmlDocument.DocumentElement.ChildNodes[intIndex].ChildNodes.Count;
        }

        public string GetSectionElementName(int intIndex)
        {
            if (intIndex < m_xmlDocument.DocumentElement.ChildNodes.Count)
                return m_xmlDocument.DocumentElement.ChildNodes[intIndex].Name;
            else
                return "";
        }
    }
}
