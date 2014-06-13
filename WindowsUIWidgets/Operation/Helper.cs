// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace AlarmWorkflow.Windows.UIWidgets.Operation
{
    internal class Helper
    {
        internal static Inline Execute(string text)
        {
            XmlDocument document = new XmlDocument();

            text = string.Format("<xml>{0}</xml>", text);

            document.PreserveWhitespace = true;
            document.LoadXml(text);
            if (document.HasChildNodes)
            {
                XmlNode firstChild = document.FirstChild;
                Inline inline = Analyse(firstChild);
                return inline;
            }
            return null;
        }

        static Inline Analyse(XmlNode node)
        {
            Span span = new Span();
            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    switch (childNode.NodeType)
                    {
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.Text:
                            switch (node.Name)
                            {
                                case "u":
                                    return new Underline(new Run(childNode.InnerText));
                                case "text":
                                    Span innerSpan = new Span(new Run(childNode.InnerText));
                                    if (node.Attributes != null)
                                    {
                                        XmlAttribute xmlAttribute = node.Attributes["size"];
                                        if (xmlAttribute != null)
                                        {
                                            innerSpan.FontSize = Convert.ToDouble(xmlAttribute.Value);
                                        }
                                        xmlAttribute = node.Attributes["color"];
                                        if (xmlAttribute != null)
                                        {
                                            BrushConverter converter = new BrushConverter();
                                            Brush brush = (Brush)converter.ConvertFromString(xmlAttribute.Value);
                                            innerSpan.Foreground = brush;
                                        }
                                    }
                                    return innerSpan;
                                case "b":
                                    return new Bold(new Run(childNode.InnerText));
                                case "i":
                                    return new Italic(new Run(childNode.InnerText));
                                default:
                                    return new Run(childNode.InnerText);
                            }
                        default:
                            switch (node.Name)
                            {
                                case "u":
                                    span.Inlines.Add(new Underline(Analyse(childNode)));
                                    break;
                                case "text":
                                    Span innerSpan = new Span(Analyse(childNode));
                                    if (node.Attributes != null)
                                    {
                                        XmlAttribute xmlAttribute = node.Attributes["size"];
                                        if (xmlAttribute != null)
                                        {
                                            innerSpan.FontSize = Convert.ToDouble(xmlAttribute.Value);
                                        }
                                        xmlAttribute = node.Attributes["color"];
                                        if (xmlAttribute != null)
                                        {
                                            BrushConverter converter = new BrushConverter();
                                            Brush brush = (Brush)converter.ConvertFromString(xmlAttribute.Value);
                                            innerSpan.Foreground = brush;
                                        }
                                    }
                                    span.Inlines.Add(innerSpan);
                                    break;
                                case "b":
                                    span.Inlines.Add(new Bold(Analyse(childNode)));
                                    break;
                                case "i":
                                    span.Inlines.Add(new Italic(Analyse(childNode)));
                                    break;
                                default:
                                    span.Inlines.Add(Analyse(childNode));
                                    break;
                            }
                            break;
                    }
                }
            }
            return span;
        }
    }
}