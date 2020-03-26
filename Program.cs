using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace lccCtcLinkAlmaUploader
{
    class lccCtcLinkAlmaUploaderClass
    {
        static lccSettingsClass lccSCSettings = new lccSettingsClass();
        static void Main(string[] lccParamALArgs)
        {
            try
            {
                lccSCSettings.lccFLogInfo(0, "[Main] STARTED");
                lccSCSettings.lccALArgs = lccParamALArgs;
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Loading command line arguments.");
                    lccFLoadArgs();
                    lccSCSettings.lccFLogInfo(0, "Loaded command line arguments.");
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Loading Logic File: "+ lccSCSettings.lccSLogicPath);
                    lccFLoadLogic(1, "", lccSCSettings.lccSLogicPath);
                    lccSCSettings.lccFLogInfo(0, "LoadedLogic File: " + lccSCSettings.lccSLogicPath);
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Processing Logic.");
                    lccFProcessLogic();
                    lccSCSettings.lccFLogInfo(0, "Processed Logic.");
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    if (lccSCSettings.lccALSourceColumnNames.Count == 0)
                    {
                        lccSCSettings.lccBAbortProgram = true;
                        lccSCSettings.lccFLogInfo(0, "Please provide at least one lcc:sourceColumnName");
                    }
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    if (lccSCSettings.lccALSourcePaths.Count == 0)
                    {
                        lccSCSettings.lccBAbortProgram = true;
                        lccSCSettings.lccFLogInfo(0, "Please provide at least one lcc:sourcePath");
                    }
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Loading Source Path(s)");
                    foreach (lccNameValueClass lccSourcePathLoop in lccSCSettings.lccALSourcePaths)
                    {
                        lccSCSettings.lccFLogInfo(0, "Loading Source -Type ["+ lccSourcePathLoop.lccSName+"] Path: "+ lccSourcePathLoop.lccSValue);
                        lccFLoadLogic(2, lccSourcePathLoop.lccSName, lccSourcePathLoop.lccSValue);
                        lccSCSettings.lccFLogInfo(0, "Loaded Source -Type [" + lccSourcePathLoop.lccSName + "] Path: " + lccSourcePathLoop.lccSValue);
                    }
                    lccSCSettings.lccFLogInfo(0, "Loaded Source Path(s)");
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Processing User Information records.");
                    lccFProcessUserInfoRecords();
                    lccSCSettings.lccFLogInfo(0, "Processed User Information records.");
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Sorting User Information records.");
                    lccFSortUserInfoRecords();
                    lccSCSettings.lccFLogInfo(0, "Sorting User Information records.");
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Invalidting User Information records.");
                    lccFInvalidateUserInfoRecords();
                    lccSCSettings.lccFLogInfo(0, "Invalidted User Information records.");
                }
                if (lccSCSettings.lccBAbortProgram == false)
                {
                    lccSCSettings.lccFLogInfo(0, "Creating XML file: "+ lccSCSettings.lccSReportPath);
                    lccFCreateXMLResults();
                    lccSCSettings.lccFLogInfo(0, "Created XML file: "+ lccSCSettings.lccSReportPath);
                }
            }
            catch (Exception lccException)
            {
                lccSCSettings.lccFLogInfo(0, "[Main] ERROR: " + lccException.Message);
            }
            lccSCSettings.lccFLogInfo(0, "[Main] COMPLETED");
            lccSCSettings.lccFLogInfo(3, "");
        }
        static public void lccFCreateXMLResults()
        {
            bool lccBUserGroupFound = false;
            int lccIUsersLoop = 0;
            int lccIUserGroupsLoop = 0;
            XmlNode lccXmlNodeRoot = null;
            XmlAttribute lccXmlAttribute = null;
            try
            {
                lccSCSettings.lccXmlResults.PrependChild(lccSCSettings.lccXmlResults.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\""));
                lccXmlNodeRoot = lccSCSettings.lccXmlResults.CreateElement("users");
                lccSCSettings.lccXmlResults.AppendChild(lccXmlNodeRoot);
                for (lccIUsersLoop = 0; lccIUsersLoop<lccSCSettings.lccALUserInfos.Count; lccIUsersLoop++)
                {
                    lccSCSettings.lccFLogInfo(1, "Skip: "+lccSCSettings.lccALUserInfos[lccIUsersLoop].lccBSkip.ToString()+"\tType: "+ lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSType+"\tPrimaryId: "+ lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSPRIMARYID+"\tLastName: "+ lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSLASTNAME + "\tUserGroup: " + lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSUSERGROUP);
                    if (lccSCSettings.lccALUserInfos[lccIUsersLoop].lccBSkip == false)
                    {
                        lccBUserGroupFound = false;
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("user");
                        //lccXmlNodeUser = lccSCSettings.lccXmlResults.CreateElement("User");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserRecordType").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("record_type");
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("desc");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserRecordTypeDesc;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserRecordType").lccXMLNode.Attributes.Append(lccXmlAttribute);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserRecordType").lccXMLNode.InnerText = lccSCSettings.lccSUserRecordType;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrimaryId").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("primary_id");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrimaryId").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSPRIMARYID;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserFirstName").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("first_name");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserFirstName").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSFIRSTNAME;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserMiddleName").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("middle_name");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserMiddleName").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSMIDDLENAME;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserLastName").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("last_name");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserLastName").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSLASTNAME;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserFullName").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("full_name");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserFullName").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSFULLNAME;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserUserGroup").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("user_group");
                        for (lccIUserGroupsLoop = 0; lccIUserGroupsLoop < lccSCSettings.lccALUserGroups.Count && lccBUserGroupFound == false; lccIUserGroupsLoop++)
                        {
                            if (lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSUSERGROUP.Equals(lccSCSettings.lccALUserGroups[lccIUserGroupsLoop].lccSName) == true)
                            {
                                lccBUserGroupFound = true;
                                lccSCSettings.lccFAccessXMLNodes(2, "UserUserGroup").lccXMLNode.InnerText = lccSCSettings.lccALUserGroups[lccIUserGroupsLoop].lccSValue;
                            }
                        }
                        if (lccBUserGroupFound == false)
                        {
                            if (lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSType.Equals("Staff")
                                || lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSType.Equals("Faculty")
                                )
                            {
                                lccSCSettings.lccFAccessXMLNodes(2, "UserUserGroup").lccXMLNode.InnerText = "FA";
                            }
                            else
                            {
                                lccSCSettings.lccFAccessXMLNodes(2, "UserUserGroup").lccXMLNode.InnerText = "ST";
                            }
                        }
                        lccSCSettings.lccFAccessXMLNodes(2, "UserPreferredLanguage").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("preferred_language");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserPreferredLanguage").lccXMLNode.InnerText = lccSCSettings.lccSUserPreferredLanguage;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserExpiryDate").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("expiry_date");
                        if (lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSEXPIRYDATE.Length > 0)
                        {
                            try
                            {
                                lccSCSettings.lccFAccessXMLNodes(2, "UserExpiryDate").lccXMLNode.InnerText = lccSCSettings.lccReturnDateString("YYYY-MM-DDZ", lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSEXPIRYDATE, 0);
                            }
                            catch (Exception lccExceptionExpiryDate)
                            {
                                lccSCSettings.lccFLogInfo(1, "Expiry Date value ["+ lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSEXPIRYDATE+"] conversion issue. ERROR: " + lccExceptionExpiryDate);
                            }
                        }

                        lccSCSettings.lccFAccessXMLNodes(2, "UserPurgeDate").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("purge_date");
                        try
                        {
                            lccSCSettings.lccFAccessXMLNodes(2, "UserPurgeDate").lccXMLNode.InnerText = lccSCSettings.lccReturnDateString("YYYY-MM-DDZ", lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSEXPIRYDATE, lccSCSettings.lccIUserExpiryDatePurgeDays);
                        }
                        catch (Exception lccExceptionUserPurgeDate)
                        {
                            lccSCSettings.lccFLogInfo(1, "Purge Date cannot be created from Expiry Date value [" + lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSEXPIRYDATE + "] conversion issue. ERROR: " + lccExceptionUserPurgeDate);
                        }

                        lccSCSettings.lccFAccessXMLNodes(2, "UserAccountType").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("account_type");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserAccountType").lccXMLNode.InnerText = lccSCSettings.lccSUserAccountType;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserExternalId").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("external_id");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserExternalId").lccXMLNode.InnerText = lccSCSettings.lccSUserExternalId;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserStatus").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("status");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserStatus").lccXMLNode.InnerText = "ACTIVE";

                        lccSCSettings.lccFAccessXMLNodes(2, "UserStatusDate").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("status_date");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserStatusDate").lccXMLNode.InnerText = lccSCSettings.lccReturnDateString("YYYY-MM-DDZ", DateTime.Now.ToString(), 0);

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfo").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("contact_info");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddresses").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("addresses");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("address");
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("preferred");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserContactInfoAddressesAddressPreferred;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.Attributes.Append(lccXmlAttribute);
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("segment_type");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserContactInfoAddressesAddresSegmentType;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.Attributes.Append(lccXmlAttribute);

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressLine1").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("line1");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressLine1").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSLINE1;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressLine2").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("line2");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressLine2").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSLINE2;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressCity").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("city");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressCity").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSCITY;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressStateProvince").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("state_province");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressStateProvince").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSSTATE;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressPostalCode").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("postal_code");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressPostalCode").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSPOSTAL;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressCountry").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("country");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressCountry").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSCOUNTRY;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressStartDate").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("start_date");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressStartDate").lccXMLNode.InnerText = lccSCSettings.lccReturnDateString("YYYY-MM-DDZ", lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSSTARTDATE, 0);

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressEndDate").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("end_date");
                        try
                        {
                            lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressEndDate").lccXMLNode.InnerText = lccSCSettings.lccReturnDateString("YYYY-MM-DDZ", lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSSTARTDATE, lccSCSettings.lccIUserContactInfoAddressesAddressEndDate);
                        }
                        catch (Exception lccExceptionUserContactInfoAddressesAddressEndDate)
                        {
                            lccSCSettings.lccFLogInfo(1, "Address End Date cannot be created from Address Start Date value [" + lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSADDRESSSTARTDATE + "] conversion issue. ERROR: " + lccExceptionUserContactInfoAddressesAddressEndDate);
                        }


                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressAddressTypes").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("address_types");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressAddressTypesAddressType").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("address_type");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressAddressTypesAddressType").lccXMLNode.InnerText = lccSCSettings.lccSUserContactInfoAddressesAddressType;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmails").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("emails");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmail").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("email");
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("preferred");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserContactInfoEmailsEmailPreferred;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmail").lccXMLNode.Attributes.Append(lccXmlAttribute);
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("segment_type");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserContactInfoEmailsEmailSegmentType;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmail").lccXMLNode.Attributes.Append(lccXmlAttribute);

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailAddress").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("email_address");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailAddress").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSEMAILADDRESS;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailTypes").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("email_types");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailTypesEmailType").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("email_type");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailTypesEmailType").lccXMLNode.InnerText = "work";

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhones").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("phones");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhone").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("phone");
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("preferred");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserContactInfoPhonesPhonePreferred;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhone").lccXMLNode.Attributes.Append(lccXmlAttribute);
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("preferred_sms");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserContactInfoPhonesPhonePreferredSms;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhone").lccXMLNode.Attributes.Append(lccXmlAttribute);
                        lccXmlAttribute = lccSCSettings.lccXmlResults.CreateAttribute("segment_type");
                        lccXmlAttribute.Value = lccSCSettings.lccSUserContactInfoPhonesPhoneSegmentType;
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhone").lccXMLNode.Attributes.Append(lccXmlAttribute);

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneNumber").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("phone_number");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneNumber").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSPHONENUMBER;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneTypes").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("phone_types");

                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneTypesPhoneType").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("phone_type");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneTypesPhoneType").lccXMLNode.InnerText = "office";

                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrefFirstName").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("pref_first_name");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrefFirstName").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSPREFFIRSTNAME;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrefMiddleName").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("pref_middle_name");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrefMiddleName").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSPREFMIDDLENAME;

                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrefLastName").lccXMLNode = lccSCSettings.lccXmlResults.CreateElement("pref_last_name");
                        lccSCSettings.lccFAccessXMLNodes(2, "UserPrefLastName").lccXMLNode.InnerText = lccSCSettings.lccALUserInfos[lccIUsersLoop].lccSPREFLASTNAME;

                        lccXmlNodeRoot.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserRecordType").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserPrimaryId").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserFirstName").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserMiddleName").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserLastName").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserFullName").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserUserGroup").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserPreferredLanguage").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserExpiryDate").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserPurgeDate").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserAccountType").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserExternalId").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserStatus").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserStatusDate").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfo").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfo").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddresses").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddresses").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressLine1").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressLine2").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressCity").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressStateProvince").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressPostalCode").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressCountry").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressStartDate").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressEndDate").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddress").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressAddressTypes").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressAddressTypes").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoAddressesAddressAddressTypesAddressType").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfo").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmails").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmails").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmail").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmail").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailAddress").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmail").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailTypes").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailTypes").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoEmailsEmailTypesEmailType").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfo").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhones").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhones").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhone").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhone").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneNumber").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhone").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneTypes").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneTypes").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserContactInfoPhonesPhonePhoneTypesPhoneType").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserPrefFirstName").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserPrefMiddleName").lccXMLNode);
                        lccSCSettings.lccFAccessXMLNodes(2, "User").lccXMLNode.AppendChild(lccSCSettings.lccFAccessXMLNodes(2, "UserPrefLastName").lccXMLNode);
                    }
                }

                //lccXmlRootNode.AppendChild(lccXmlSubNode);
                lccSCSettings.lccXmlResults.Save(lccSCSettings.lccSReportPath);
            }
            catch (Exception lccException)
            {
                lccSCSettings.lccFLogInfo(0, "[lccFCreateXMLResults] ERROR: " + lccException.Message);
            }
        }
        static public bool lccFProcessLogic()
        {
           
            bool lccBReturnVal = false;
            int lccIRecordsLoop = 0;
            List<string[]> lccALRecords = null;
            try
            {
                    lccALRecords = lccSCSettings.lccALLogicRecords;
                for (lccIRecordsLoop = 0; lccIRecordsLoop < lccALRecords.Count; lccIRecordsLoop++)
                {
                    if (lccALRecords[lccIRecordsLoop].Length > 1)
                    {
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:logPath") == 0)
                        {
                            lccSCSettings.lccSLogPath = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:reportPath") == 0)
                        {
                            lccSCSettings.lccSReportPath = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:sourceColumnName") == 0)
                        {
                            lccSCSettings.lccALSourceColumnNames.Add(lccALRecords[lccIRecordsLoop][1]);
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userRecordTypeDesc") == 0)
                        {
                            lccSCSettings.lccSUserRecordTypeDesc = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userRecordType") == 0)
                        {
                            lccSCSettings.lccSUserRecordType = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userPreferredLanguage") == 0)
                        {
                            lccSCSettings.lccSUserPreferredLanguage = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userAccountType") == 0)
                        {
                            lccSCSettings.lccSUserAccountType = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userExternalId") == 0)
                        {
                            lccSCSettings.lccSUserExternalId = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoAddressesAddressPreferred") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoAddressesAddressPreferred = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoAddressesAddressSegmentType") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoAddressesAddresSegmentType = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoAddressesAddressType") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoAddressesAddressType = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoEmailsEmailPreferred") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoEmailsEmailPreferred = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoEmailsEmailSegmentType") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoEmailsEmailSegmentType = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoPhonesPhonePreferred") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoPhonesPhonePreferred = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoPhonesPhonePreferredSms") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoPhonesPhonePreferredSms = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoPhonesPhoneSegmentType") == 0)
                        {
                            lccSCSettings.lccSUserContactInfoPhonesPhoneSegmentType = lccALRecords[lccIRecordsLoop][1];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userExpiryDatePurgeDays") == 0)
                        {
                            try
                            {
                                lccSCSettings.lccIUserExpiryDatePurgeDays = int.Parse(lccALRecords[lccIRecordsLoop][1]);
                            }
                            catch (Exception lccExceptionUserExpiryDatePurgeDays)
                            {
                                lccSCSettings.lccFLogInfo(1, lccALRecords[lccIRecordsLoop][0] + " value [" + lccALRecords[lccIRecordsLoop][1] + "] issue converting to number.  Reverting to default value [" + lccSCSettings.lccIUserExpiryDatePurgeDays.ToString() + "]. ERROR: " + lccExceptionUserExpiryDatePurgeDays.Message);
                            }
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userContactInfoAddressesAddressEndDate") == 0)
                        {
                            try
                            {
                                lccSCSettings.lccIUserContactInfoAddressesAddressEndDate = int.Parse(lccALRecords[lccIRecordsLoop][1]);
                            }
                            catch (Exception lccExceptionUserContactInfoAddressesAddressEndDate)
                            {
                                lccSCSettings.lccFLogInfo(1, lccALRecords[lccIRecordsLoop][0] + " value [" + lccALRecords[lccIRecordsLoop][1] + "] issue converting to number.  Reverting to default value [" + lccSCSettings.lccIUserContactInfoAddressesAddressEndDate.ToString() + "]. ERROR: " + lccExceptionUserContactInfoAddressesAddressEndDate.Message);
                            }
                        }
                    }
                    if (lccALRecords[lccIRecordsLoop].Length > 2)
                    {
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:sourcePath") == 0)
                        {
                            lccSCSettings.lccALSourcePaths.Add(new lccNameValueClass());
                            lccSCSettings.lccALSourcePaths[lccSCSettings.lccALSourcePaths.Count - 1].lccSName = lccALRecords[lccIRecordsLoop][1];
                            lccSCSettings.lccALSourcePaths[lccSCSettings.lccALSourcePaths.Count - 1].lccSValue = lccALRecords[lccIRecordsLoop][2];
                        }
                        if (lccALRecords[lccIRecordsLoop][0].CompareTo("lcc:userGroup") == 0)
                        {
                            lccSCSettings.lccALUserGroups.Add(new lccNameValueClass());
                            lccSCSettings.lccALUserGroups[lccSCSettings.lccALUserGroups.Count - 1].lccSName = lccALRecords[lccIRecordsLoop][1];
                            lccSCSettings.lccALUserGroups[lccSCSettings.lccALUserGroups.Count - 1].lccSValue = lccALRecords[lccIRecordsLoop][2];
                        }
                    }
                }
                lccBReturnVal = true;
            }
            catch (Exception lccException)
            {
                lccSCSettings.lccFLogInfo(0, "[lccFProcessLogic] ERROR: " + lccException.Message);
            }
            return lccBReturnVal;
        }
        static public bool lccFInvalidateUserInfoRecords()
        {
            List<lccNameValueClass> lccALTypesFound = new List<lccNameValueClass>();
            bool lccBReturnVal = false;
            int lccIRecordsLoop = 0;
            int lccIDuplicates = 0;
            int lccIStartDuplicateSection = -1;
            int lccIDuplicatesLoop = 0;
            int lccIDuplicateSetsLoop = 0;
            try
            {
                lccALTypesFound.Add(new lccNameValueClass());
                lccALTypesFound[lccALTypesFound.Count - 1].lccSName = "Employee";
                lccALTypesFound.Add(new lccNameValueClass());
                lccALTypesFound[lccALTypesFound.Count - 1].lccSName = "Student";

                lccSCSettings.lccALStartEndDuplicateSets.Clear();
                for (lccIRecordsLoop = 0; lccIRecordsLoop < lccSCSettings.lccALUserInfos.Count-1; lccIRecordsLoop++)
                {
                    if (lccSCSettings.lccALUserInfos[lccIRecordsLoop].lccSPRIMARYID.Equals(lccSCSettings.lccALUserInfos[lccIRecordsLoop + 1].lccSPRIMARYID) == true)
                    {
                        lccIDuplicates++;
                        if (lccIStartDuplicateSection == -1)
                        {
                            lccIStartDuplicateSection = lccIRecordsLoop;
                            lccSCSettings.lccALStartEndDuplicateSets.Add(new lccStartEndSetClass());
                            lccSCSettings.lccALStartEndDuplicateSets[lccSCSettings.lccALStartEndDuplicateSets.Count - 1].lccIStart = lccIRecordsLoop;
                        }
                        lccSCSettings.lccALStartEndDuplicateSets[lccSCSettings.lccALStartEndDuplicateSets.Count - 1].lccIEnd = lccIRecordsLoop+1;
                    }
                    else
                    {
                        if (lccIStartDuplicateSection != -1)
                        {
                            lccIDuplicates = 0;
                            lccIStartDuplicateSection = -1;
                        }
                    }
                }
                for (lccIDuplicateSetsLoop = 0; lccIDuplicateSetsLoop < lccSCSettings.lccALStartEndDuplicateSets.Count; lccIDuplicateSetsLoop++)
                {
                    lccALTypesFound[0].lccSValue = "";
                    lccALTypesFound[1].lccSValue = "";
                    for (lccIDuplicatesLoop= lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIStart; lccIDuplicatesLoop<= lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIEnd; lccIDuplicatesLoop++)
                    {
                        lccSCSettings.lccFLogInfo(1, "Duplicate Skip [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccBSkip.ToString() + "] Type [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSType + "] PrimaryId [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSPRIMARYID + "] Group [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSUSERGROUP + "]");
                        if (lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSType.Equals("Staff") == true
                            || lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSType.Equals("Faculty") == true
                            )
                        {
                            lccALTypesFound[0].lccSValue = "YES";
                            lccSCSettings.lccFLogInfo(1, "lccALTypesFound for Employee set to YES");
                        }
                        else if (lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSType.Equals("Students") == true)
                        {
                            lccALTypesFound[1].lccSValue = "YES";
                            lccSCSettings.lccFLogInfo(1, "lccALTypesFound for Student set to YES");
                        }
                    }
                    if (lccALTypesFound[0].lccSValue.Length > 0
                        && lccALTypesFound[1].lccSValue.Length > 0
                        )
                    {
                        for (lccIDuplicatesLoop = lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIStart; lccIDuplicatesLoop <= lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIEnd; lccIDuplicatesLoop++)
                        {
                            if (lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccBSkip == false)
                            {
                                if (lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSType.Equals("Students") == true)
                                {
                                    lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccBSkip = true;
                                    lccSCSettings.lccFLogInfo(1, "Employee and Student Duplicate, removing student, skipping: Skip [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccBSkip.ToString() + "] Type [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSType + "] PrimaryId [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSPRIMARYID + "] Group [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSUSERGROUP + "]");
                                }
                            }
                        }
                    }
                    for (lccIDuplicatesLoop = lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIStart; lccIDuplicatesLoop < lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIEnd; lccIDuplicatesLoop++)
                    {
                        if (lccSCSettings.lccALUserInfos[lccIDuplicatesLoop + 1].lccBSkip == false)
                        {
                            lccSCSettings.lccALUserInfos[lccIDuplicatesLoop + 1].lccBSkip = true;
                            lccSCSettings.lccFLogInfo(1, "Duplicate Id, skipping: Skip [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop + 1].lccBSkip.ToString() + "] Type [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop + 1].lccSType + "] PrimaryId [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop + 1].lccSPRIMARYID + "] Group [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop + 1].lccSUSERGROUP + "]");
                        }
                    }
                    for (lccIDuplicatesLoop = lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIStart; lccIDuplicatesLoop <= lccSCSettings.lccALStartEndDuplicateSets[lccIDuplicateSetsLoop].lccIEnd; lccIDuplicatesLoop++)
                    {
                        lccSCSettings.lccFLogInfo(1, "Final Duplicate Skip [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccBSkip.ToString() + "] Type [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSType + "] PrimaryId [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSPRIMARYID + "] Group [" + lccSCSettings.lccALUserInfos[lccIDuplicatesLoop].lccSUSERGROUP + "]");
                    }
                }

            }
            catch (Exception lccException)
            {
                lccSCSettings.lccFLogInfo(0, "[lccFInvalidateUserInfoRecords] ERROR: " + lccException.Message);
            }
            return lccBReturnVal;
        }
        static public bool lccFSortUserInfoRecords()
        {
            bool lccBReturnVal = false;
            try
            {
                lccSCSettings.lccALUserInfos.Sort(delegate (lccUserInfoClass lccRecord1, lccUserInfoClass lccRecord2) { return lccFSortUserInfos(lccRecord1, lccRecord2); });
                lccBReturnVal = true;
            }
            catch (Exception lccException)
            {
                lccSCSettings.lccFLogInfo(0, "[lccFSortUserInfoRecords] ERROR: " + lccException.Message);
            }
            return lccBReturnVal;
        }
        static int lccFSortUserInfos(lccUserInfoClass lccParamRecord1, lccUserInfoClass lccParamRecord2)
        {
            int lccIReturn = 0;
            try
            {
                lccIReturn = lccParamRecord1.lccSPRIMARYID.CompareTo(lccParamRecord2.lccSPRIMARYID);
                if (lccIReturn == 0)
                {
                    lccIReturn = lccParamRecord1.lccSType.CompareTo(lccParamRecord2.lccSType);
                }
                if (lccIReturn == 0)
                {
                    lccIReturn = lccParamRecord1.lccSUSERGROUP.CompareTo(lccParamRecord2.lccSUSERGROUP);
                }
            }
            catch (Exception lccException)
            {
                lccSCSettings.lccFLogInfo(0, "[lccFSortUserInfos] ERROR: " + lccException.Message);
            }
            return lccIReturn;
        }
        static public bool lccFProcessUserInfoRecords()
        {
            bool lccBReturnVal = false;
            int lccIRecordsLoop = 0;
            int lccIColumnsLoop = 0;
            int lccIColumnNamesLoop = 0;
            int lccIOnRecord = 0;
            StringBuilder lccSBOutput = new StringBuilder();
            try
            {
                for (lccIRecordsLoop = 0; lccIRecordsLoop < lccSCSettings.lccALUsersInfoRecords.Count; lccIRecordsLoop++)
                {
                    lccIOnRecord++;
                    if (lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns.Length != lccSCSettings.lccALSourceColumnNames.Count)
                    {
                        lccSBOutput.Append("Record ["+ lccIOnRecord.ToString()+"] columns["+ lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns.Length + "] does not have the correct number of columns ["+lccSCSettings.lccALSourceColumnNames.Count.ToString  ()+"]: ");
                        for (lccIColumnsLoop = 0; lccIColumnsLoop < lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns.Length; lccIColumnsLoop++)
                        {
                            if (lccIColumnsLoop > 0)
                            {
                                lccSBOutput.Append("\t");
                            }
                            lccSBOutput.Append(lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop]);
                        }
                        lccSBOutput.Append("\r\n");
                        lccSCSettings.lccFLogInfo(0, lccSBOutput.ToString());
                    }
                    else
                    {
                        lccSCSettings.lccALUserInfos.Add(new lccUserInfoClass());
                        lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSType = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccSType;
                        for (lccIColumnsLoop = 0; lccIColumnsLoop < lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns.Length; lccIColumnsLoop++)
                        {
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("PRIMARY-ID") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSPRIMARYID = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("FIRST-NAME") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSFIRSTNAME = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("MIDDLE-NAME") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSMIDDLENAME = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("LAST-NAME") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSLASTNAME = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("FULL-NAME") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSFULLNAME = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("USER-GROUP") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSUSERGROUP = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("EXPIRY-DATE") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSEXPIRYDATE = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-LINE1") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSLINE1 = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-LINE2") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSLINE2 = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-CITY") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSCITY = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-STATE") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSSTATE = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-POSTAL") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSPOSTAL = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-COUNTRY") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSCOUNTRY = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-START-DATE") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSSTARTDATE = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("ADDRESS-TYPE") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSADDRESSTYPE = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("EMAIL-ADDRESS") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSEMAILADDRESS = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("PHONE-NUMBER") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSPHONENUMBER = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("PREF-FIRST-NAME") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSPREFFIRSTNAME = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("PREF-MIDDLE-NAME") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSPREFMIDDLENAME = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }
                            if (lccSCSettings.lccALSourceColumnNames[lccIColumnsLoop].Equals("PREF-LAST-NAME") == true)
                            {
                                lccSCSettings.lccALUserInfos[lccSCSettings.lccALUserInfos.Count - 1].lccSPREFLASTNAME = lccSCSettings.lccALUsersInfoRecords[lccIRecordsLoop].lccALColumns[lccIColumnsLoop];
                            }

                        }
                        //lccSCSettings.lccALSourcePaths[lccSCSettings.lccALSourcePaths.Count - 1].lccSValue = lccALRecords[lccIRecordsLoop].lccALColumns[2];
                    }
                }
                lccBReturnVal = true;
            }
            catch (Exception lccException)
            {
                lccSCSettings.lccFLogInfo(0, "[lccFProcessUserInfoRecords] ERROR: " + lccException.Message);
            }
            return lccBReturnVal;
        }
        static public bool lccFLoadLogic(int lccParamIFlag, string lccParamSType, string lccParamSPath)
        {
            // lccParamIFlag
            // 1 - regular
            // 2 - users info
            bool lccBReturnVal = false;
            FileStream lccFSSourceFile = null;
            StreamReader lccSRSourceFile = null;
            String lccSSource = "";
            try
            {
                if (lccParamSPath.Length == 0)
                {
                    lccSCSettings.lccBAbortProgram = true;
                    if (lccParamIFlag == 1)
                    {
                        lccSCSettings.lccFLogInfo(2, "[lccFLoadLogic] Please provide lcc:logicPath");
                    }
                    if (lccParamIFlag == 2)
                    {
                        lccSCSettings.lccFLogInfo(2, "[lccFLoadLogic] Please provide lcc:sourcePath");
                    }

                }
                else if (File.Exists(lccParamSPath) == false)
                {
                    lccSCSettings.lccBAbortProgram = true;
                    if (lccParamIFlag == 1)
                    {
                        lccSCSettings.lccFLogInfo(2, "[lccFLoadLogic] ERROR: lcc:logicPath [" + lccParamSPath + "] could not be read.");
                    }
                    if (lccParamIFlag == 2)
                    {
                        lccSCSettings.lccFLogInfo(2, "[lccFLoadLogic] ERROR: lcc:sourcePath [" + lccParamSPath + "] could not be read.");
                    }
                }
                else
                {
                    lccFSSourceFile = new FileStream(lccParamSPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    lccSRSourceFile = new StreamReader(lccFSSourceFile);
                    while ((lccSSource = lccSRSourceFile.ReadLine()) != null)
                    {
                        if (lccParamIFlag == 1)
                        {
                            lccSCSettings.lccALLogicRecords.Add(lccSSource.Split('\t'));
                        }
                        if (lccParamIFlag == 2)
                        {
                            lccSCSettings.lccALUsersInfoRecords.Add(new lccUserInfoRecordsClass());
                            lccSCSettings.lccALUsersInfoRecords[lccSCSettings.lccALUsersInfoRecords.Count - 1].lccSType = lccParamSType;
                            lccSCSettings.lccALUsersInfoRecords[lccSCSettings.lccALUsersInfoRecords.Count - 1].lccALColumns = lccSSource.Split('\t');
                        }
                    }
                    lccSRSourceFile.Close();
                    lccFSSourceFile.Close();
                }
                lccBReturnVal = true;
            }
            catch (Exception lccException)
            {
                lccSCSettings.lccBAbortProgram = true;
                lccSCSettings.lccFLogInfo(0, "[lccFLoadLogic] ERROR: " + lccException.Message);
            }
            return lccBReturnVal;
        }
        static public bool lccFLoadArgs()
        {
            bool lccBReturn = false;
            int lccILoop = 0;
            int lccIOnArg = 0;
            try
            {
                for (lccILoop = 0; lccILoop < lccSCSettings.lccALArgs.Length; lccILoop++)
                {
                    if (lccSCSettings.lccALArgs[lccILoop].Equals("lcc:logicPath") == true)
                    {
                        lccIOnArg = 1;
                    }
                    else
                    {
                        switch (lccIOnArg)
                        {
                            case 1:
                                lccSCSettings.lccSLogicPath = lccSCSettings.lccALArgs[lccILoop];
                                break;
                        }
                        lccIOnArg = 0;
                    }
                }
            }
            catch (Exception lccException)
            {
                lccFLogInfo(2, "[lccFLoadArgs] ERROR: " + lccException.Message);
            }
            return lccBReturn;
        }

        static public bool lccFLogInfo(int lccIFlag, String logStr)
        {
            // lccIFlag
            // 0 - console and log
            // 1 - only write to log
            // 2 - console only
            // 3 - flush
            bool lccBReturnVal = false;
            string lccSLogFilePath = "";
            StringBuilder lccSBLogAppendYearMonthStr = new StringBuilder();
            StringBuilder lccSBTargetRecord = new StringBuilder();
            StringBuilder lccSBLogConsole = new StringBuilder();
            FileStream lccFSLogFile = null;
            StreamWriter lccSWLogFile = null;
            FileShare lccFSFileShare = FileShare.ReadWrite;
            try
            {
                switch (lccIFlag)
                {
                    case 3:
                        if (lccSCSettings.lccSLogPath.Length > 0)
                        {
                            lccSBLogAppendYearMonthStr.Append(DateTime.Now.Year.ToString());
                            if (DateTime.Now.Month < 10)
                            {
                                lccSBLogAppendYearMonthStr.Append("0");
                            }
                            lccSBLogAppendYearMonthStr.Append(DateTime.Now.Month.ToString());
                            if (DateTime.Now.Day < 10)
                            {
                                lccSBLogAppendYearMonthStr.Append("0");
                            }
                            lccSBLogAppendYearMonthStr.Append(DateTime.Now.Day.ToString());
                            lccSLogFilePath = lccSCSettings.lccSLogPath + "-" + lccSBLogAppendYearMonthStr.ToString()+ ".log";
                        }
                        break;
                }

                if (lccIFlag == 3)
                {
                    lccFSLogFile = new FileStream(lccSLogFilePath, FileMode.Append, FileAccess.Write, lccFSFileShare);
                    lccSWLogFile = new StreamWriter(lccFSLogFile);
                }


                else
                {
                    lccSBTargetRecord.Append(lccSCSettings.lccReturnDateString("YYYYMMDD", DateTime.Now.ToString(), 0));
                    lccSBLogConsole.Append(lccSCSettings.lccReturnDateString("HH:MM:SS.MS", DateTime.Now.ToString(), 0));
                    lccSBTargetRecord.Append("\t");
                    lccSBLogConsole.Append("\t");
                    switch (lccIFlag)
                    {
                        case 0:
                        case 1:
                            break;
                    }
                    lccSBTargetRecord.Append(logStr);
                    lccSBLogConsole.Append(logStr);
                    switch (lccIFlag)
                    {
                        case 0:
                        case 2:
                            Console.WriteLine(lccSBLogConsole.ToString());
                            break;
                    }
                    if (lccIFlag == 0
                        || lccIFlag == 1
                        )
                    {
                        lccSCSettings.lccALLogRecords.Add(lccSBTargetRecord.ToString());
                    }
                }
                if (lccIFlag == 3)
                {
                    if (lccSCSettings.lccSLogPath.Length > 0)
                    {
                        foreach (string lccSLogLoop in lccSCSettings.lccALLogRecords)
                        {
                            lccSWLogFile.WriteLine(lccSLogLoop);
                        }
                        lccSCSettings.lccALLogRecords.Clear();
                        lccSWLogFile.Close();
                        lccFSLogFile.Close();
                    }
                }
                lccBReturnVal = true;
            }
            catch
            {
            }
            return lccBReturnVal;
        }
    }
    public class lccXMLNodeClass
    {
        public string lccSId = "";
        public XmlNode lccXMLNode = null;

        public lccXMLNodeClass()
        {
            lccFClearValues();
        }
        public void lccFClearValues()
        {
            lccSId = "";
            lccXMLNode = null;
        }
    }
    public class lccNameValueClass
    {
        public string lccSName { get; set; }
        public string lccSValue { get; set; }
        public lccNameValueClass()
        {
            lccFClearValues();
        }
        public void lccFClearValues()
        {
            lccSName = "";
            lccSValue = "";
        }
    }
    public class lccUserInfoClass
    {
        public bool lccBSkip = false;
        public string lccSType = "";
        public string lccSPRIMARYID = "";
        public string lccSFIRSTNAME = "";
        public string lccSMIDDLENAME = "";
        public string lccSLASTNAME = "";
        public string lccSFULLNAME = "";
        public string lccSUSERGROUP = "";
        public string lccSEXPIRYDATE = "";
        public string lccSADDRESSLINE1 = "";
        public string lccSADDRESSLINE2 = "";
        public string lccSADDRESSCITY = "";
        public string lccSADDRESSSTATE = "";
        public string lccSADDRESSPOSTAL = "";
        public string lccSADDRESSCOUNTRY = "";
        public string lccSADDRESSSTARTDATE = "";
        public string lccSADDRESSTYPE = "";
        public string lccSEMAILADDRESS = "";
        public string lccSPHONENUMBER = "";
        public string lccSPREFFIRSTNAME = "";
        public string lccSPREFMIDDLENAME = "";
        public string lccSPREFLASTNAME = "";

        public lccUserInfoClass()
        {
            lccFClearValues();
        }
        public void lccFClearValues()
        {
            lccBSkip = false;
            lccSType = "";
            lccSPRIMARYID = "";
            lccSFIRSTNAME = "";
            lccSMIDDLENAME = "";
            lccSLASTNAME = "";
            lccSFULLNAME = "";
            lccSUSERGROUP = "";
            lccSEXPIRYDATE = "";
            lccSADDRESSLINE1 = "";
            lccSADDRESSLINE2 = "";
            lccSADDRESSCITY = "";
            lccSADDRESSSTATE = "";
            lccSADDRESSPOSTAL = "";
            lccSADDRESSCOUNTRY = "";
            lccSADDRESSSTARTDATE = "";
            lccSADDRESSTYPE = "";
            lccSEMAILADDRESS = "";
            lccSPHONENUMBER = "";
            lccSPREFFIRSTNAME = "";
            lccSPREFMIDDLENAME = "";
            lccSPREFLASTNAME = "";
        }
    }
    public class lccStartEndSetClass
    {
        public int lccIStart = 0;
        public int lccIEnd = 0;
        public lccStartEndSetClass()
        {
            lccFClearValues();
        }
        public void lccFClearValues()
        {
        lccIStart = 0;
        lccIEnd = 0;
        }
    }
    public class lccUserInfoRecordsClass
    {
        public string lccSType = "";
        public string[] lccALColumns = null;
        public lccUserInfoRecordsClass()
        {
            lccFClearValues();
        }
        public void lccFClearValues()
        {
            lccSType = "";
            lccALColumns = null;
        }
    }
    public class lccSettingsClass
    {
        public bool lccBAbortProgram = false;
        public int lccIUserExpiryDatePurgeDays = 0;
        public int lccIUserContactInfoAddressesAddressEndDate = 0;
        public string lccSLogicPath = "";
        public string lccSLogPath = "";
        public string lccSReportPath = "";
        public string[] lccALArgs = null;
        public string lccSUserRecordTypeDesc = "";
        public string lccSUserRecordType = "";
        public string lccSUserPreferredLanguage = "";
        public string lccSUserAccountType = "";
        public string lccSUserExternalId = "";
        public string lccSUserContactInfoAddressesAddressPreferred = "";
        public string lccSUserContactInfoAddressesAddresSegmentType = "";
        public string lccSUserContactInfoAddressesAddressType = "";
        public string lccSUserContactInfoEmailsEmailPreferred = "";
        public string lccSUserContactInfoEmailsEmailSegmentType = "";
        public string lccSUserContactInfoPhonesPhonePreferred = "";
        public string lccSUserContactInfoPhonesPhonePreferredSms = "";
        public string lccSUserContactInfoPhonesPhoneSegmentType = "";
        public List<lccNameValueClass> lccALUserGroups = new List<lccNameValueClass>();
        public List<string> lccALLogRecords = new List<string>();
        public XmlDocument lccXmlResults = new XmlDocument();
        public List<string[]> lccALLogicRecords = new List<string[]>();
        public List<lccUserInfoRecordsClass> lccALUsersInfoRecords = new List<lccUserInfoRecordsClass>();
        public List<string> lccALSourceColumnNames = new List<string>();
        public List<lccNameValueClass> lccALSourcePaths = new List<lccNameValueClass>();
        public List<lccUserInfoClass> lccALUserInfos = new List<lccUserInfoClass>();
        public List<lccStartEndSetClass> lccALStartEndDuplicateSets = new List<lccStartEndSetClass>();
        public List<lccXMLNodeClass> lccALXMLNodes = new List<lccXMLNodeClass>();

        public lccXMLNodeClass lccFAccessXMLNodes(int lccParamIFlag, string lccParamSId)
        {
            // lccParamIFlag
            // 1 - add
            // 2 - retrieve
            int lccILoop = 0;
            lccXMLNodeClass lccXmlNodeReturn = null;
            try
            {
                if (lccParamIFlag == 2)
                {
                    lccALXMLNodes.Add(new lccXMLNodeClass());
                    lccALXMLNodes[lccALXMLNodes.Count - 1].lccSId = lccParamSId;
                }
                if (lccParamIFlag == 2)
                {
                    for (lccILoop = 0; lccILoop < lccALXMLNodes.Count && lccXmlNodeReturn == null; lccILoop++)
                    {
                        if (lccALXMLNodes[lccILoop].lccSId.Equals(lccParamSId) == true)
                        {
                            lccXmlNodeReturn = lccALXMLNodes[lccILoop];
                        }
                    }
                }
            }
            catch (Exception lccException)
            {
                lccFLogInfo(0, "[lccFAccessXMLNodes] ERROR: " + lccException.Message);
            }
            return lccXmlNodeReturn;
        }
        public bool lccFLogInfo(int lccIFlag, String logStr)
        {
            // lccIFlag
            // 0 - console and log
            // 1 - only write to log
            // 2 - console only
            // 3 - flush
            bool lccBReturnVal = false;
            string lccSLogFilePath = "";
            StringBuilder lccSBLogAppendYearMonthStr = new StringBuilder();
            StringBuilder lccSBTargetRecord = new StringBuilder();
            StringBuilder lccSBLogConsole = new StringBuilder();
            FileStream lccFSLogFile = null;
            StreamWriter lccSWLogFile = null;
            FileShare lccFSFileShare = FileShare.ReadWrite;
            try
            {
                switch (lccIFlag)
                {
                    case 3:
                        if (lccSLogPath.Length > 0)
                        {
                            lccSBLogAppendYearMonthStr.Append(DateTime.Now.Year.ToString());
                            if (DateTime.Now.Month < 10)
                            {
                                lccSBLogAppendYearMonthStr.Append("0");
                            }
                            lccSBLogAppendYearMonthStr.Append(DateTime.Now.Month.ToString());
                            if (DateTime.Now.Day < 10)
                            {
                                lccSBLogAppendYearMonthStr.Append("0");
                            }
                            lccSBLogAppendYearMonthStr.Append(DateTime.Now.Day.ToString());
                            lccSLogFilePath = lccSLogPath + "-" + lccSBLogAppendYearMonthStr.ToString() + ".log";
                        }
                        break;
                }

                if (lccIFlag == 3)
                {
                    lccFSLogFile = new FileStream(lccSLogFilePath, FileMode.Append, FileAccess.Write, lccFSFileShare);
                    lccSWLogFile = new StreamWriter(lccFSLogFile);
                }


                else
                {
                    lccSBTargetRecord.Append(lccReturnDateString("YYYYMMDD", DateTime.Now.ToString(), 0));
                    lccSBLogConsole.Append(lccReturnDateString("HH:MM:SS.MS", DateTime.Now.ToString(), 0));
                    lccSBTargetRecord.Append("\t");
                    lccSBLogConsole.Append("\t");
                    switch (lccIFlag)
                    {
                        case 0:
                        case 1:
                            break;
                    }
                    lccSBTargetRecord.Append(logStr);
                    lccSBLogConsole.Append(logStr);
                    switch (lccIFlag)
                    {
                        case 0:
                        case 2:
                            Console.WriteLine(lccSBLogConsole.ToString());
                            break;
                    }
                    if (lccIFlag == 0
                        || lccIFlag == 1
                        )
                    {
                        lccALLogRecords.Add(lccSBTargetRecord.ToString());
                    }
                }
                if (lccIFlag == 3)
                {
                    if (lccSLogPath.Length > 0)
                    {
                        foreach (string lccSLogLoop in lccALLogRecords)
                        {
                            lccSWLogFile.WriteLine(lccSLogLoop);
                        }
                        lccALLogRecords.Clear();
                        lccSWLogFile.Close();
                        lccFSLogFile.Close();
                    }
                }
                lccBReturnVal = true;
            }
            catch
            {
            }
            return lccBReturnVal;
        }
        public string lccReturnDateString(string lccParamSFlag, string lccParamSDateTime, int lccParamIDaysAdjust)
        {
            int lccIFlag = -1;
            // lccIFlag
            // 0 - return YYYYMM
            // 1 - return YYYYMM    [tab]   HH:MM:SS.MS
            // 2 - return YYYYMMDDHHMMSS
            // 3 - return YYYYMMDD
            // 4 - return YYYYMMDDHHMMSSMS
            // 5 - return HHMMSSMS
            // 6 - return YYMMDD
            // 7 - YY
            // 8 - return YYYY-MM-DD
            // 9 - return YYYY-MM-DDZ
            // 10 - return HH:MM:SS.MS
            string lccSReturnVal = "";
            DateTime lccDTNow = DateTime.Now;
            try
            {
                try
                {
                    lccDTNow = Convert.ToDateTime(lccParamSDateTime);
                }
                catch (Exception lccExceptionDateTimeConvert)
                {
                    lccFLogInfo(1, "[lccReturnDateString] Error on value ["+ lccParamSDateTime+"], using current Date/Time.  ERROR: " + lccExceptionDateTimeConvert.Message);
                    lccDTNow = DateTime.Now;
                }
                if (lccParamIDaysAdjust != 0)
                {
                    lccDTNow = lccDTNow.AddDays(lccParamIDaysAdjust);
                }
                if (lccParamSFlag.Equals("YYYYMM") == true)
                {
                    lccIFlag = 0;
                }
                else if (lccParamSFlag.Equals("YYYYMM[tab]HH:MM:SS:MS") == true)
                {
                    lccIFlag = 1;
                }
                else if (lccParamSFlag.Equals("YYYYMMDDHHMMSS") == true)
                {
                    lccIFlag = 2;
                }
                else if (lccParamSFlag.Equals("YYYYMMDD") == true)
                {
                    lccIFlag = 3;
                }
                else if (lccParamSFlag.Equals("YYYYMMDDHHMMSSMS") == true)
                {
                    lccIFlag = 4;
                }
                else if (lccParamSFlag.Equals("HHMMSSMS") == true)
                {
                    lccIFlag = 5;
                }
                else if (lccParamSFlag.Equals("YYMMDD") == true)
                {
                    lccIFlag = 6;
                }
                else if (lccParamSFlag.Equals("YY") == true)
                {
                    lccIFlag = 7;
                }
                else if (lccParamSFlag.Equals("YYYY-MM-DD") == true)
                {
                    lccIFlag = 8;
                }
                else if (lccParamSFlag.Equals("YYYY-MM-DDZ") == true)
                {
                    lccIFlag = 9;
                }
                else if (lccParamSFlag.Equals("HH:MM:SS.MS") == true)
                {
                    lccIFlag = 10;
                }
                switch (lccIFlag)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        if (lccIFlag == 6)
                        {
                            lccSReturnVal = lccDTNow.Year.ToString().Substring(2, 2);
                        }
                        else if (lccIFlag == 7)
                        {
                            lccSReturnVal = lccDTNow.Year.ToString().Substring(2, 2);
                        }
                        else
                        {
                            lccSReturnVal = lccDTNow.Year.ToString();
                        }
                        if (lccIFlag == 8
                            || lccIFlag == 9
                            )
                        {
                            lccSReturnVal += "-";
                        }
                        if (lccIFlag != 7)
                        {
                            if (lccDTNow.Month < 10)
                            {
                                lccSReturnVal += "0";
                            }
                            lccSReturnVal += lccDTNow.Month.ToString();
                        }
                        break;
                }
                switch (lccIFlag)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 6:
                    case 8:
                    case 9:
                        if (lccIFlag == 8
                            || lccIFlag == 9
                            )
                        {
                            lccSReturnVal += "-";
                        }
                        if (lccDTNow.Day < 10)
                        {
                            lccSReturnVal += "0";
                        }
                        lccSReturnVal += lccDTNow.Day.ToString();
                        if (lccIFlag == 8
                            || lccIFlag == 9
                            )
                        {
                            lccSReturnVal += "Z";
                        }
                        break;
                }
                switch (lccIFlag)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                    case 10:

                        if (lccDTNow.Hour < 10)
                        {
                            lccSReturnVal += "0";
                        }
                        lccSReturnVal += lccDTNow.Hour.ToString();
                        if (lccIFlag == 1
                            || lccIFlag == 10
                            )
                        {
                            lccSReturnVal += ":";
                        }
                        if (lccDTNow.Minute < 10)
                        {
                            lccSReturnVal += "0";
                        }
                        lccSReturnVal += lccDTNow.Minute.ToString();
                        if (lccIFlag == 1
                            || lccIFlag == 10
                            )
                        {
                            lccSReturnVal += ":";
                        }
                        if (lccDTNow.Second < 10)
                        {
                            lccSReturnVal += "0";
                        }
                        lccSReturnVal += lccDTNow.Second.ToString();
                        break;
                }
                switch (lccIFlag)
                {

                    case 4:
                    case 5:
                    case 10:
                        if (lccIFlag == 1
                            || lccIFlag == 10)
                        {
                            lccSReturnVal += ".";
                        }
                        if (lccDTNow.Millisecond < 10)
                        {
                            lccSReturnVal += "0";
                        }
                        lccSReturnVal += lccDTNow.Millisecond.ToString();
                        break;
                }
            }
            catch (Exception lccException)
            {
                lccFLogInfo(0, "[lccReturnDateString] ERROR: Reading File.\r\n");
                lccFLogInfo(0, "[lccReturnDateString]  Message: " + lccException.Message + "\r\n");
            }
            return lccSReturnVal;
        }
        public lccSettingsClass()
        {
            lccFClearValues();
        }
        public void lccFClearValues()
        {
            lccBAbortProgram = false;
            lccIUserExpiryDatePurgeDays = 100;
            lccIUserContactInfoAddressesAddressEndDate = 100;
            lccSLogicPath = "lccCtcLinkAlmaUploader-logic.txt";
            lccSLogPath = "lccCtcLinkAlmaUploader";
            lccSReportPath = "lccCtcLinkAlmaUploader-report.xml";
            lccSUserRecordTypeDesc = "Public";
            lccSUserRecordType = "PUBLIC";
            lccSUserPreferredLanguage = "";
            lccSUserAccountType = "";
            lccSUserExternalId = "";
            lccSUserContactInfoAddressesAddressPreferred = "";
            lccSUserContactInfoAddressesAddresSegmentType = "";
            lccSUserContactInfoAddressesAddressType = "";
            lccSUserContactInfoEmailsEmailPreferred = "";
            lccSUserContactInfoEmailsEmailSegmentType = "";
            lccSUserContactInfoPhonesPhonePreferred = "";
            lccSUserContactInfoPhonesPhonePreferredSms = "";
            lccSUserContactInfoPhonesPhoneSegmentType = "";
            lccALArgs = null;
            lccALUserGroups.Clear();
            lccALLogicRecords.Clear();
            lccALUsersInfoRecords.Clear();
            lccALSourceColumnNames.Clear();
            lccALSourcePaths.Clear();
            lccALUserInfos.Clear();
            lccALLogRecords.Clear();
            lccALStartEndDuplicateSets.Clear();
            lccALXMLNodes.Clear();
            lccFAccessXMLNodes(1, "User");
            lccFAccessXMLNodes(1, "UserRecordType");
            lccFAccessXMLNodes(1, "UserPrimaryId");
            lccFAccessXMLNodes(1, "UserFirstName");
            lccFAccessXMLNodes(1, "UserMiddleName");
            lccFAccessXMLNodes(1, "UserLastName");
            lccFAccessXMLNodes(1, "UserFullName");
            lccFAccessXMLNodes(1, "UserUserGroup");
            lccFAccessXMLNodes(1, "UserPreferredLanguage");
            lccFAccessXMLNodes(1, "UserExpiryDate");
            lccFAccessXMLNodes(1, "UserPurgeDate");
            lccFAccessXMLNodes(1, "UserAccountType");
            lccFAccessXMLNodes(1, "UserExternalId");
            lccFAccessXMLNodes(1, "UserStatus");
            lccFAccessXMLNodes(1, "UserStatusDate");
            lccFAccessXMLNodes(1, "UserContactInfo");
            lccFAccessXMLNodes(1, "UserContactInfoAddresses");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddress");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressLine1");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressLine2");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressCity");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressStateProvince");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressPostalCode");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressCountry");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressStartDate");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressEndDate");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressAddressTypes");
            lccFAccessXMLNodes(1, "UserContactInfoAddressesAddressAddressTypesAddressType");
            lccFAccessXMLNodes(1, "UserContactInfoEmails");
            lccFAccessXMLNodes(1, "UserContactInfoEmailsEmail");
            lccFAccessXMLNodes(1, "UserContactInfoEmailsEmailAddress");
            lccFAccessXMLNodes(1, "UserContactInfoEmailsEmailTypes");
            lccFAccessXMLNodes(1, "UserContactInfoEmailsEmailTypesEmailType");
            lccFAccessXMLNodes(1, "UserContactInfoPhones");
            lccFAccessXMLNodes(1, "UserContactInfoPhonesPhone");
            lccFAccessXMLNodes(1, "UserContactInfoPhonesPhonePhoneNumber");
            lccFAccessXMLNodes(1, "UserContactInfoPhonesPhonePhoneTypes");
            lccFAccessXMLNodes(1, "UserContactInfoPhonesPhonePhoneTypesPhoneType");
            lccFAccessXMLNodes(1, "UserPrefFirstName");
            lccFAccessXMLNodes(1, "UserPrefMiddleName");
            lccFAccessXMLNodes(1, "UserPrefLastName");

        }
    }
}
