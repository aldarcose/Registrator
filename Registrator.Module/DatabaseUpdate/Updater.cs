using System;
using System.Linq;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;

using System.Reflection;
using System.IO;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Dictionaries;
using DevExpress.Persistent.BaseImpl;

namespace Registrator.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppUpdatingModuleUpdatertopic
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            SecurityRoles();

            // DB initial update:
            string dirPath = @"c:\Users\tsb\Documents\Projects\Registrator\file30\";

            /* Сначала инициализируем классификаторы, справочники, элементарные объекты */
            
            // эти классификаторы, справочники, реестры не используем. Используем их переименованные копии.
            #region F, R, V Classifiers
            // R dictionaries
            /*
            Registrator.Module.BusinessObjects.Dictionaries.R001.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "R001.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.R002.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "R002.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.R003.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "R003.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.R004.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "R004.xml"));
            //*/

            // F dictionaries
            /*
            Registrator.Module.BusinessObjects.Dictionaries.F005.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F005.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F006.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F006.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F007.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F007.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F008.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F008.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F009.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F009.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F010.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F010.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F011.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F011(УДЛ).xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F012.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F012.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F014.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F014.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.F015.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "F015.xml"));
            //*/
            
            // V dictionaries
            /*
            Registrator.Module.BusinessObjects.Dictionaries.V002.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V002.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V003.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V003.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V004.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V004_231112.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V005.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V005.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V006.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V006.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V008.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V008.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V009.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V009.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V010.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V010.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V012.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V012.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V013.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V013.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.V014.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V014.xml"));
            //*/
            #endregion

            // Константы
            var regionConstant = "CurrentRegion";
            Constants constant = ObjectSpace.FindObject<Constants>(new BinaryOperator("Name", regionConstant));
            if (constant == null)
            {
                constant = ObjectSpace.CreateObject<Constants>();
                constant.Name = regionConstant;
                constant.Value = "03";
            }

            var currentMOCode = "CurrentMOCode";
            constant = ObjectSpace.FindObject<Constants>(new BinaryOperator("Name", currentMOCode));
            if (constant == null)
            {
                constant = ObjectSpace.CreateObject<Constants>();
                constant.Name = currentMOCode;
                constant.Value = "032021";
            }

            var dsTarifConstant = "DnevnoyStacionarTarif";
            constant = ObjectSpace.FindObject<Constants>(new BinaryOperator("Name", dsTarifConstant));
            if (constant == null)
            {
                constant = ObjectSpace.CreateObject<Constants>();
                constant.Name = dsTarifConstant;
                constant.Value = "7599.32";
            }


            
            //*/
            //Registrator.Module.BusinessObjects.Dictionaries.Medicament.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"apteka\apteka.xml"));
            //Registrator.Module.BusinessObjects.Country.UpdateDbFromXml(ObjectSpace, Path.Combine(dirPath, @"oksm_data.xml"));
            // мед. примитивы
            /*
            Registrator.Module.BusinessObjects.Dictionaries.VidMedVmeshatelstva.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V001.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.VidUsloviyOkazMedPomoshi.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V006.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.ResultatObrasheniya.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V009.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.SposobOplatiMedPom.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "V010.xml"));
             * 
            Registrator.Module.BusinessObjects.Dictionaries.MedProfil.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"PROFIL.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.TipStacionara.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\hospitalization\type_stac.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.ClinicStatGroups.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\hospitalization\ksg.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.FormaMedPomoshi.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"FOR_POM.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.ResultatDispanserizacii.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"RSLT_D.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.VidUslugiMedPomoshi.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"USL_OK.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.VidMedPomoshi.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"VIDPOM.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.VidDispanserizacii.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"DISP.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.VidPolisa.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"VPOLIS.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.IshodZabolevaniya.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"ISHOD.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.MedProfil.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"PROFIL.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.ClinicStatGroups.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\hospitalization\ksg.xml"));
            //Registrator.Module.BusinessObjects.Dictionaries.MKB10.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, "codifiers_reestr_mkb_tab.xml"));
            // */
            //Registrator.Module.BusinessObjects.Dictionaries.TerritorialUsluga.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"территориальные услуги\общий.xml"));


            // примитивы доктора
            /*
            Registrator.Module.BusinessObjects.Dictionaries.DoctorDolgnost.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\dolgnost.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.DoctorUslugi.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\usl.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.DoctorSpec.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"PRVS.xml"));
            // */
            
            // представление специальностей в виде дерева: поле HIGH указывает на уровень иерархии.
            //DoctorSpecTree.GetTree(ObjectSpace, ObjectSpace.GetObjects<DoctorSpec>().ToList(), null);

            // примитивы пациента
            /*
            Registrator.Module.BusinessObjects.Dictionaries.Kategoriya.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\kategoriya.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.KategoriyaLgot.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\kateg_lgot.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.Lgota.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\lgota.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.SocStatus.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\sozstatus.xml"));
            Registrator.Module.BusinessObjects.Dictionaries.VidDocumenta.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"F011(УДЛ).xml"));
            */
            // справочники МО и СМО
            /*
            Registrator.Module.BusinessObjects.MedOrg.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"Реестр МО_041213.xml"));
            Registrator.Module.BusinessObjects.StrahMedOrg.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"SMO.xml"));
            //*/


            /* затем инициализируем комплексные объекты */

            // справочники данных больницы
            /*
            Registrator.Module.BusinessObjects.Otdel.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"codifiers_otdel_tab.xml"));
            Registrator.Module.BusinessObjects.Uchastok.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\uchastok.xml"));
            Registrator.Module.BusinessObjects.Doctor.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"from_bb\doctor.xml"));

            //*/
            //Registrator.Module.BusinessObjects.Pacient.UpdateDbFromXml(this.ObjectSpace, Path.Combine(dirPath, @"SRZ.xml"));
        }


        /*private void UpdateXMLPacient(string resourceName)
        {
            XDocument doc = XDocument.Load(resourceName);

            UnitOfWork uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
            int counter = 0;

            foreach (XElement el in doc.Root.Element("ROWDATA").Elements("ROW"))
            {
                string fam = el.Attribute("FAM").Value;
                string nam = el.Attribute("IM").Value;
                string ot = el.Attribute("OT").Value;
                Pacient pacient = uow.FindObject<Pacient>(CriteriaOperator.Parse("Fam=? AND Nam=? AND Ot=?", fam, nam, ot));
                if (pacient == null)
                {
                    pacient = new Pacient(uow);
                    pacient.Fam = (el.Attribute("FAM").Value);
                    pacient.Nam = el.Attribute("IM").Value;
                    pacient.Ot = el.Attribute("OT").Value;
                    pacient.DR = el.Attribute("DR").Value == "" ? new DateTime(1900,1,1) : Convert.ToDateTime(el.Attribute("DR").Value);
                    pacient.Gender = el.Attribute("W").Value =="1" ? Gender.Мужской: Gender.Женский;

                    pacient.Address.OKATO =el.Attribute("RN").Value;
                    pacient.Address.OKATOP = el.Attribute("PRN").Value;
                    pacient.Address.Street = el.Attribute("UL").Value;
                    pacient.Address.House = el.Attribute("DOM").Value;
                    pacient.Address.Build = el.Attribute("KOR").Value;
                    pacient.Address.Flat = el.Attribute("KV").Value;

                    pacient.Document.Type = uow.FindObject<VidDocumenta>(CriteriaOperator.Parse("Code=?", el.Attribute("DOCTP").Value));
                    pacient.Document.Serial = el.Attribute("DOCS").Value;
                    pacient.Document.Number = el.Attribute("DOCN").Value;

                    var polis = new Polis(uow);
                    polis.Type = uow.FindObject<VidPolisa>(CriteriaOperator.Parse("Code=?", el.Attribute("OPDOC").Value));
                    polis.Serial = el.Attribute("SPOL").Value;
                    polis.Number = el.Attribute("NPOL").Value;
                    pacient.Polises.Add(polis);
                    counter++;
                }
                else
                {
                    if (pacient.Polises.Count == 0)
                    {
                        var polis = new Polis(uow);
                        polis.Type = uow.FindObject<VidPolisa>(CriteriaOperator.Parse("Code=?", el.Attribute("OPDOC").Value));
                        polis.Serial = el.Attribute("SPOL").Value;
                        polis.Number = el.Attribute("NPOL").Value;
                        pacient.Polises.Add(polis);
                        counter++;
                    }
                }

                if (counter % 1000 == 0)
                {
                    uow.CommitChanges();
                    uow.Dispose();
                    uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                }
            }
        }*/

        private void SecurityRoles()
        {
            DoctorRole adminDoctorRole = ObjectSpace.FindObject<DoctorRole>(new BinaryOperator("Name", SecurityStrategy.AdministratorRoleName));
            if (adminDoctorRole == null)
            {
                adminDoctorRole = ObjectSpace.CreateObject<DoctorRole>();
                adminDoctorRole.Name = SecurityStrategy.AdministratorRoleName;
                adminDoctorRole.IsAdministrative = true;
                adminDoctorRole.Save();
            }
            Doctor admin = ObjectSpace.FindObject<Doctor>(new BinaryOperator("UserName", "Admin"));
            if (admin == null)
            {
                admin = ObjectSpace.CreateObject<Doctor>();
                admin.UserName = "Admin";
                admin.SetPassword("");
                admin.DoctorRoles.Add(adminDoctorRole);
            }
            else
            {
                admin.DoctorRoles.Add(adminDoctorRole);
                admin.SetPassword("p@ssworD");
            }

            Doctor dvp = ObjectSpace.FindObject<Doctor>(new BinaryOperator("UserName", "dvp"));
            if (dvp == null)
            {
                dvp = ObjectSpace.CreateObject<Doctor>();
                dvp.UserName = "dvp";
                dvp.SetPassword("");
                dvp.DoctorRoles.Add(adminDoctorRole);
            }

            /*
            
            var doctors = ObjectSpace.GetObjects<Doctor>().Cast<Doctor>().ToList();
            int i = 0;
            foreach (var doctor in doctors)
            {
                if (doctor.UserName.Equals("Admin") == false)
                {
                    doctor.IsActive = false;
                    doctor.ChangePasswordOnFirstLogon = true;
                    doctor.UserName = "User" + doctor.InnerCode.ToString();
                    if (doctor.UserName.Equals(""))
                    {
                        doctor.UserName = string.Format("User{0}", i++);
                    }
                }   
            }*/

            

            /*
            SecuritySystemUser sampleUser = ObjectSpace.FindObject<SecuritySystemUser>(new BinaryOperator("UserName", "User"));
            if (sampleUser == null)
            {
                sampleUser = ObjectSpace.CreateObject<SecuritySystemUser>();
                sampleUser.UserName = "User";
                sampleUser.SetPassword("");
            }
            SecuritySystemRole defaultRole = CreateDefaultRole();
            sampleUser.Roles.Add(defaultRole);

            SecuritySystemUser userAdmin = ObjectSpace.FindObject<SecuritySystemUser>(new BinaryOperator("UserName", "Admin"));
            if (userAdmin == null)
            {
                userAdmin = ObjectSpace.CreateObject<SecuritySystemUser>();
                userAdmin.UserName = "Admin";
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("");
            }
            // If a role with the Administrators name doesn't exist in the database, create this role
            SecuritySystemRole adminRole = ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null)
            {
                adminRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                adminRole.Name = "Administrators";
            }
            adminRole.IsAdministrative = true;
            userAdmin.Roles.Add(adminRole);*/

            ObjectSpace.CommitChanges();
        }
        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
        private SecuritySystemRole CreateDefaultRole() {
            SecuritySystemRole defaultRole = ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name", "Default"));
            if(defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                defaultRole.Name = "Default";

                defaultRole.AddObjectAccessPermission<SecuritySystemUser>("[Oid] = CurrentUserId()", SecurityOperations.ReadOnlyAccess);
                defaultRole.AddMemberAccessPermission<SecuritySystemUser>("ChangePasswordOnFirstLogon", SecurityOperations.Write);
                defaultRole.AddMemberAccessPermission<SecuritySystemUser>("StoredPassword", SecurityOperations.Write);
                defaultRole.SetTypePermissionsRecursively<SecuritySystemRole>(SecurityOperations.Read, SecuritySystemModifier.Allow);
                defaultRole.SetTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecuritySystemModifier.Allow);
                defaultRole.SetTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecuritySystemModifier.Allow);
            }
            return defaultRole;
        }
    }
}
