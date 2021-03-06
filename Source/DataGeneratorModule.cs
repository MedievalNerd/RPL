using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomExperiment
{
    public class DataGeneratorModule : PartModule
    {
        #region Variables & Fields

        [KSPField(isPersistant = true)]
        public bool IsEnabled;

        [KSPField(isPersistant = false)]
        public bool AlwaysActive;

        [KSPField(isPersistant = false)]
        public String InputResource;

        [KSPField(isPersistant = false)]
        public String OutputResource;

        [KSPField(isPersistant = false)]
        public float InputRatio;

        [KSPField(isPersistant = false)]
        public float OutputRatio;

        [KSPField(isPersistant = false)]
        public float MaxDataAmount;

        [KSPField(isPersistant = false)]
        public String TargetBody;

        [KSPField(isPersistant = false)]
        public String TargetSituation1;

        [KSPField(isPersistant = false)]
        public String TargetSituation2;

        [KSPField(isPersistant = false)]
        public String TargetSituation3;

        #endregion

        #region Events & Actions

        [KSPEvent(guiActive = true, guiName = "Activate Recorder", active = true)]
        public void ActivateConverter()
        {
            IsEnabled = true;
        }

        [KSPEvent(guiActive = true, guiName = "Deactivate Recorder", active = false)]
        public void DeactivateConverter()
        {
            IsEnabled = false;
        }

        [KSPAction("Activate Recorder")]
        public void ActivateConverterAction(KSPActionParam param)
        {
            ActivateConverter();
        }

        [KSPAction("Deactivate Recorder")]
        public void DeactivateConverterAction(KSPActionParam param)
        {
            DeactivateConverter();
        }

        [KSPAction("Toggle Recorder")]
        public void ToggleConverterAction(KSPActionParam param)
        {
            IsEnabled = !IsEnabled;
        }

        #endregion

        #region Private Methods



        private bool CheckBody()
        {
            if (vessel.mainBody.name == TargetBody)
                return true;

            return false;
        }

        private bool CheckMaxData()
        {
            PartResource Data = this.part.Resources.list.Find(
                delegate(PartResource pr)
                {
                    return pr.resourceName == OutputResource;
                }
            );

            if (!(Data == null) && Data.amount >= MaxDataAmount)
                return true;


            return false;
        }

        private bool CheckElectricCharge()
        {
            double totalAmount = 0;

            foreach (PartResource pr in GetConnectedResources(this.part, InputResource))
            {
                totalAmount += pr.amount;

                if (totalAmount >= 1)
                    return true;
            }

            return false;
        }

        private List<PartResource> GetConnectedResources(Part part, String resourceName)
        {
            List<PartResource> resources = new List<PartResource>();
            PartResourceDefinition resDef = PartResourceLibrary.Instance.GetDefinition(resourceName);
            part.GetConnectedResources(resDef.id, resDef.resourceFlowMode, resources);
            return resources;
        }

        // Ethernet's Methods for ExperimentSituations (Below this point)

        private ExperimentSituations GetScienceSituation(Vessel vessel)
        {
            return GetScienceSituation(vessel.altitude, vessel.situation, vessel.mainBody);
        }

        public ExperimentSituations GetScienceSituation(double altitude, Vessel.Situations situation, CelestialBody body)
        {

            CelestialBodyScienceParams pars = body.scienceValues;

            ScreenMessages.PostScreenMessage(String.Format("{0} flyingAltitudeThreshold : {1}", body.name, pars.flyingAltitudeThreshold), 3, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(String.Format("{0} spaceAltitudeThreshold : {1}", body.name, pars.spaceAltitudeThreshold), 3, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(String.Format("{0} atmosphereScaleHeight : {1}", body.name, body.atmosphereScaleHeight), 3, ScreenMessageStyle.UPPER_CENTER);

            if (situation == Vessel.Situations.LANDED)
                return ExperimentSituations.SrfLanded;
            else if (situation == Vessel.Situations.SPLASHED)
                return ExperimentSituations.SrfSplashed;
            else if (altitude <= pars.flyingAltitudeThreshold)
                return ExperimentSituations.FlyingLow;
            else if (altitude <= body.atmosphereScaleHeight * 1000 * 13.8) // -ln(10^-6)
                return ExperimentSituations.FlyingHigh;
            else if (altitude <= pars.spaceAltitudeThreshold)
                return ExperimentSituations.InSpaceLow;
            else
                return ExperimentSituations.InSpaceHigh;

            // original method with atmospheric check.
            //if (situation == Vessel.Situations.LANDED)
            //    return ExperimentSituations.SrfLanded;
            //else if (situation == Vessel.Situations.SPLASHED)
            //    return ExperimentSituations.SrfSplashed;
            //else if (body.atmosphere && altitude <= pars.flyingAltitudeThreshold)
            //    return ExperimentSituations.FlyingLow;
            //else if (body.atmosphere && altitude <= body.atmosphereScaleHeight * 1000 * 13.8) // -ln(10^-6)
            //    return ExperimentSituations.FlyingHigh;
            //else if (altitude <= pars.spaceAltitudeThreshold)
            //    return ExperimentSituations.InSpaceLow;
            //else
            //    return ExperimentSituations.InSpaceHigh;

        }

        // Ethernet's Methods for ExperimentSituations (Above this point)

        // START Confirm Situation Method

        public bool ConfirmLocation(ExperimentSituations experimentlocation)
        {
            string currentSituation = "Placeholder";

            if (experimentlocation == ExperimentSituations.SrfLanded)
            {
                currentSituation = "SrfLanded";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);

                if (TargetSituation1 == currentSituation || TargetSituation2 == currentSituation || TargetSituation3 == currentSituation)
                {
                    return true;
                }

                if (TargetSituation1 != currentSituation && TargetSituation2 != currentSituation && TargetSituation3 != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.SrfSplashed)
            {
                currentSituation = "SrfSplashed";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);

                if (TargetSituation1 == currentSituation || TargetSituation2 == currentSituation || TargetSituation3 == currentSituation)
                {
                    return true;
                }

                if (TargetSituation1 != currentSituation && TargetSituation2 != currentSituation && TargetSituation3 != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.FlyingLow)
            {
                currentSituation = "FlyingLow";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == currentSituation || TargetSituation2 == currentSituation || TargetSituation3 == currentSituation)
                {
                    return true;
                }

                if (TargetSituation1 != currentSituation && TargetSituation2 != currentSituation && TargetSituation3 != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.FlyingHigh)
            {
                currentSituation = "FlyingHigh";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == currentSituation || TargetSituation2 == currentSituation || TargetSituation3 == currentSituation)
                {
                    return true;
                }

                if (TargetSituation1 != currentSituation && TargetSituation2 != currentSituation && TargetSituation3 != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.InSpaceLow)
            {
                currentSituation = "InSpaceLow";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == currentSituation || TargetSituation2 == currentSituation || TargetSituation3 == currentSituation)
                {
                    return true;
                }

                if (TargetSituation1 != currentSituation && TargetSituation2 != currentSituation && TargetSituation3 != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.InSpaceHigh)
            {
                currentSituation = "InSpaceHigh";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);

                if (TargetSituation1 == currentSituation || TargetSituation2 == currentSituation || TargetSituation3 == currentSituation)
                {
                    return true;
                }

                if (TargetSituation1 != currentSituation && TargetSituation2 != currentSituation && TargetSituation3 != currentSituation)
                {
                    return false;
                }
            }

            return false;

        }

        // END Confirm Situation Method

        #endregion

        #region Public Methods

        public override void OnFixedUpdate()
        {
            if (!IsEnabled && !AlwaysActive)
                return;

            if (IsEnabled && CheckElectricCharge() == false)
            {
                DeactivateConverter();
                ScreenMessages.PostScreenMessage(String.Format("Warning! Insufficient Power to run the Data Recorder!"), 3, ScreenMessageStyle.UPPER_CENTER);
                return;
            }


            // Inverted Body and situation for debugging must replace after.
            if (IsEnabled && ConfirmLocation(GetScienceSituation(vessel)) == false)
            {
                DeactivateConverter();
                ScreenMessages.PostScreenMessage(String.Format("Warning! The Data Recorder is only calibrated for {0},{1},{2} situations only!", TargetSituation1, TargetSituation2, TargetSituation3), 3, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            if (IsEnabled && CheckBody() == false)
            {
                DeactivateConverter();

                ScreenMessages.PostScreenMessage(String.Format("Warning! The Data Recorder is calibrated for {0} only!", TargetBody), 3, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            if (IsEnabled && CheckMaxData())
            {
                DeactivateConverter();
                ScreenMessages.PostScreenMessage(String.Format("Warning! The Data Recorder is full!"), 3, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            this.part.RequestResource(InputResource, InputRatio);
            this.part.RequestResource(OutputResource, -OutputRatio);

        }

        public override void OnStart(PartModule.StartState state)
        {
            Actions["ActivateConverterAction"].guiName = Events["ActivateConverter"].guiName = String.Format("Activate Data Recorder");
            Actions["DeactivateConverterAction"].guiName = Events["DeactivateConverter"].guiName = String.Format("Deactivate Data Recorder");
            Actions["ToggleConverterAction"].guiName = String.Format("Toggle Data Recorder");

            Events["ActivateConverter"].guiActive = Events["DeactivateConverter"].guiActive = !AlwaysActive;

            if (state == StartState.Editor) { return; }

            this.part.force_activate();
        }

        public override void OnUpdate()
        {

            Events["ActivateConverter"].active = !IsEnabled;
            Events["DeactivateConverter"].active = IsEnabled;



        }


        // Debug Target Situation
        //ScreenMessages.PostScreenMessage(String.Format("TargetSituation :{0}", TargetSituation),3, ScreenMessageStyle.UPPER_CENTER);
        // Biome tracking
        //ScreenMessages.PostScreenMessage(String.Format("Current Biome :{0}", getExperimentBiome(vessel.mainBody, vessel.latitude, vessel.longitude)), 3, ScreenMessageStyle.UPPER_CENTER);
        // NEW STUFF
        //private string getExperimentBiome(CelestialBody body, double lat, double lon)
        //{
        //    return body.BiomeMap.GetAtt(lat * 0.01745329238474369, lon * 0.01745329238474369).name;
        //}
        // NEW STUFF



        #endregion
    }
}