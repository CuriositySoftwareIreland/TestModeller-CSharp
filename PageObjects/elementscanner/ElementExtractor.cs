using CuriositySoftware.PageObjects.Entities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace CuriositySoftware.PageObjects.ElementScanner
{
    public class ElementExtractor {
        public ElementExtractor()
        {

        }

        public static Boolean updateParameter(PageObjectParameterEntity parameter, IWebElement element, IWebDriver driver)
        {
            // If the identifer no longer works -> We need to update it in the framework
            // Also need to assign a confidence value which gets tracked upwards
            String newIdentifier = null;
            if (parameter.paramType.Equals(VipAutomationSelectorEnum.ClassName)) {
                newIdentifier = element.GetAttribute("class");

            } else if (parameter.paramType.Equals(VipAutomationSelectorEnum.Id)) {
                newIdentifier = element.GetAttribute("id");

            } else if (parameter.paramType.Equals(VipAutomationSelectorEnum.LinkText)) {
                newIdentifier = element.Text;

            } else if (parameter.paramType.Equals(VipAutomationSelectorEnum.Name)) {
                newIdentifier = element.GetAttribute("name");

            } else if (parameter.paramType.Equals(VipAutomationSelectorEnum.TagName)) {
                newIdentifier = element.TagName;

            } else if (parameter.paramType.Equals(VipAutomationSelectorEnum.XPath)) {
                newIdentifier = GetElementXPath(element, driver);

            } else {
                Console.WriteLine("Unsupported identifier reusing old value - " + parameter.paramType);

                newIdentifier = parameter.paramValue;
            }

            if (newIdentifier != null && newIdentifier.Length > 0) {
                parameter.paramValue = (newIdentifier);


                By identifer = GetElementIdentifierForParameter(parameter);
                if (identifer == null) {
                    parameter.confidence = (0);

                    return true;
                }

                try {
                    List<IWebElement> elem = new List<IWebElement>(driver.FindElements(identifer));

                    if (elem.Count == 0) {
                        parameter.confidence = (0);
                    } else {
                        parameter.confidence = (1.0f / elem.Count);
                    }
                } catch (Exception e) {
                    parameter.confidence = (0);
                }

                return true;
            } else {
                parameter.confidence = (0);

                return false;
            }
        }

        public static By GetElementIdentifierForParameter(PageObjectParameterEntity parameterEntity)
        {
            if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.ClassName)) {
                return By.ClassName(parameterEntity.paramValue);

            } else if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.CssSelector)) {
                return By.CssSelector(parameterEntity.paramValue);

            } else if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.Id)) {
                return By.Id(parameterEntity.paramValue);

            } else if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.LinkText)) {
                return By.LinkText(parameterEntity.paramValue);

            } else if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.Name)) {
                return By.Name(parameterEntity.paramValue);

            } else if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.PartialLinkText)) {
                return By.PartialLinkText(parameterEntity.paramValue);

            } else if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.TagName)) {
                return By.TagName(parameterEntity.paramValue);

            } else if (parameterEntity.paramType.Equals(VipAutomationSelectorEnum.XPath)) {
                return By.XPath(parameterEntity.paramValue);

            } else {
                return null;
            }
        }

        public static String GetElementXPath(IWebElement element, IWebDriver driver)
        {
            return (String)((IJavaScriptExecutor)driver).ExecuteScript(
                    "getXPath=function(node)" +
                            "{" +
                            "if (node.id !== '')" +
                            "{" +
                            "return '//' + node.tagName.toLowerCase() + '[@id=\"' + node.id + '\"]'" +
                            "}" +

                            "if (node === document.body)" +
                            "{" +
                            "return node.tagName.toLowerCase()" +
                            "}" +

                            "var nodeCount = 0;" +
                            "var childNodes = node.parentNode.childNodes;" +

                            "for (var i=0; i<childNodes.length; i++)" +
                            "{" +
                            "var currentNode = childNodes[i];" +

                            "if (currentNode === node)" +
                            "{" +
                            "return getXPath(node.parentNode) + '/' + node.tagName.toLowerCase() + '[' + (nodeCount+1) + ']'" +
                    "}" +

                    "if (currentNode.nodeType === 1 && " +
                        "currentNode.tagName.toLowerCase() === node.tagName.toLowerCase())" +
                    "{" +
                        "nodeCount++" +
                    "}" +
                    "}" +
                    "};" +

                    "return getXPath(arguments[0]);", element);
        }
    }
}