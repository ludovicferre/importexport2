using Altiris.NS.Exceptions;
using Altiris.NS.ItemManagement;
using Altiris.NS.Logging;
using Altiris.NS.Security;
using Altiris.NS.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace Symantec.CWoC {
    public class ImportExport2 {

        public const int E_SUCCESS = 0;
        public const int E_ARGSINVALID = -1;        // Incorrect command line arguments provided
        public const int E_ACCESSDENIED = -2;       // User must be in the Altiris Admin group
        public const int E_ITEMERROR = -3;          // Error running item import/export
        public const int E_FOLDERINVALID = -4;      // Error the folder does not contain file thisFolder.xml
        public const int E_FOLDERERROR = -5;        // Error running folder import/export
        public const int E_SECURITYCONTEXT = -6;    // Failed to set security context
        public const int E_ITEMEXISTS = -7;         // Item exist, not importing (incremental import only)

        public static int Main(string[] args) {
            StringBuilder cmd_line = new StringBuilder();
            foreach (string arg in args)
                cmd_line.Append(arg + " ");
            string msg = string.Format("AeXImportExport being run with command line:{0}", cmd_line.ToString());

            if (args.Length == 0) {
            } else if (args[0] == "/import" && args.Length == 2) {
                log_event(msg);
                return import(args[1], false);
            } else if (args[0] == "/import" && args.Length == 3) {
                log_event(msg);
                return import(args[1], args[2] == "/incremental" ? true : false );
            } else if (args[0] == "/export" && args.Length == 3) {
                log_event(msg);
                return export(args[1], args[2]);
            }
            Console.WriteLine(help_message);
            return E_ARGSINVALID;
        }

        private static void log_event(string text) {
            Console.WriteLine(text);
            EventLog.ReportInfo(text);
        }

        private static int import(string import_path, bool incremental) {
            try {
                SecurityContextManager.SetContextData();
                if (!is_admin()) {
                    log_event(permission_error_msg);
                    return E_ACCESSDENIED;
                }
                if (import_path.EndsWith(".xml")) {
                    try {
                        if (incremental) {
                            Guid itemGuid = new Guid(import_path.Replace(".xml", ""));
                            IItem item = Item.GetItem(itemGuid);
                            if (item != null) {
                                log_event(string.Format("Item with guid {0} [{1}] already exist. We will not import.", itemGuid, item.Name));
                                return E_ITEMEXISTS;
                            }
                        }
                        Item.ImportItemFromFile(import_path);
                        log_event(string.Format("Item {0} imported successfully.", import_path));
                        return E_SUCCESS;
                    } catch (Exception e) {
                        log_event(string.Format("Failed to import the item from the file [{0}]. Error:{1}", import_path, e.Message));
                        return E_ITEMERROR;
                    }
                } else {
                    try {
                        DirectoryInfo info = new DirectoryInfo(import_path);
                        if (File.Exists(Path.Combine(info.FullName, "thisFolder.xml"))) {
                            Console.WriteLine(folder_import_warning);
                            Folder.ImportFolder(import_path);
                            log_event(string.Format("Folder {0} imported successfully", info.FullName));
                            return E_SUCCESS;
                        } else {
                            log_event(string.Format("Failed to import folder [{0}]. The folder must have thisFolder.xml file in it.", import_path));
                            return E_FOLDERINVALID;
                        }
                    } catch (Exception e) {
                        log_event(string.Format("Failed to import folder [{0}]", import_path, e.Message));
                        return E_FOLDERERROR;
                    }
                }
            } catch (Exception e) {
                log_event(e.Message);
                return E_SECURITYCONTEXT;
            }
        }

        private static int export(string export_guid, string export_path) {
            try {
                SecurityContextManager.SetContextData();
                if (!is_admin()) {
                    log_event(permission_error_msg);
                    return E_ACCESSDENIED;
                }
                try {
                    Guid itemGuid = new Guid(export_guid);
                    Folder folder = Item.GetItem(itemGuid) as Folder;
                    if (folder != null) {
                        Console.WriteLine(folder_export_warning);
                        folder.ExportFolder(export_path);
                        log_event(string.Format("Folder Guid {0} exported successfully.\n", itemGuid.ToString()));
                        return E_SUCCESS;
                    }
                    IItem item = Item.GetItem(itemGuid);
                    if (item != null) {
                        string full_path = export_path + @"\" + item.Guid.ToString() + ".xml";
                        using (StreamWriter writer = File.CreateText(full_path)) {
                            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                            writer.WriteLine(item.Export());
                            log_event(string.Format("Item Guid {0} exported successfully", itemGuid.ToString()));
                        }
                        return E_SUCCESS;
                    }
                    return E_ITEMERROR;
                } catch (Exception e) {
                    log_event(string.Format("Failed to export item {0}. Error:{1} ", export_guid, e.Message));
                    return E_ITEMERROR;
                }
            } catch (Exception e) {
                log_event(e.Message);
                return E_SECURITYCONTEXT;
            }
        }

        private static bool is_admin() {
            try {
                string identity = string.Empty;
                Role role = SecurityRoleManager.Get(new Guid("{2E1F478A-4986-4223-9D1E-B5920A63AB41}"));
                if (role != null)
                    identity = role.Trustee.Identity;

                if (identity != string.Empty) {
                    foreach (string str2 in SecurityTrusteeManager.GetCurrentUserMemberships()) {
                        if (str2 == identity)
                            return true;
                    }
                    using (WindowsIdentity id = WindowsIdentity.GetCurrent()) {
                        log_event(string.Format("The current user {0} is not a member of the Altiris Administrators group.", id.Name));
                    }
                }
            } catch {
                log_event("Failed to get Altiris Administrators role. The database may not be configured yet.");
            }
            return false;
        }

        private const string help_message = @"
To import or export, use one of the following commands:
    /import <Import path to either item.xml file OR directory >
    /import <Import path to either item.xml file OR directory > /incremental
    /export <Item OR Folder GUID to export> <Destination folder path>
";
        private const string folder_export_warning = @"
The Export folder task might take a long time, depending on the complexity of the folder structure.
Please be patient when exporting large amount of data...
";
        private const string folder_import_warning = @"
The Import folder task might take a long time, depending on the complexity of the folder structure.
Please be patient when importing large amount of data...
";
        private const string permission_error_msg = @"
Failed to run Import/Export Utility due to inadequate permissions. User should be a member of Altiris Administrators!";
    }
}
