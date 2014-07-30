using System;
using System.Collections.Generic;

namespace CustomExperiment
{
    public class ChemicalReleaseModule : PartModule
    {
        [KSPField(isPersistant = true)]
        public bool IsEnabled;
        [KSPField(isPersistant = false)]
        public bool AlwaysActive;
        [KSPField(isPersistant = false)]
        public string InputResource;
        [KSPField(isPersistant = false)]
        public string OutputResource;
        [KSPField(isPersistant = false)]
        public float InputRatio;
        [KSPField(isPersistant = false)]
        public float OutputRatio;
        [KSPField(isPersistant = false)]
        public float MaxDataAmount;
        [KSPField(isPersistant = false)]
        public string TargetBody;
        [KSPField(isPersistant = false)]
        public string TargetSituation1;
        [KSPField(isPersistant = false)]
        public string TargetSituation2;
        [KSPField(isPersistant = false)]
        public string TargetSituation3;
        [KSPEvent(guiActive = true, guiName = "Release Chemical", active = true)]
        public void ActivateConverter()
        {
            IsEnabled = true;
        }
        [KSPEvent(guiActive = true, guiName = "Deactivate Chemical", active = false)]
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
        private bool CheckBody()
        {
            return base.vessel.mainBody.name == TargetBody;
        }
        private bool CheckMaxData()
        {
            PartResource partResource = base.part.Resources.list.Find((PartResource pr) => pr.resourceName == OutputResource);
            return !(partResource == null) && partResource.amount >= (double)MaxDataAmount;
        }
        private bool CheckElectricCharge()
        {
            double num = 0.0;
            bool result;
            foreach (PartResource current in GetConnectedResources(base.part, InputResource))
            {
                num += current.amount;
                if (num >= 1.0)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
        private List<PartResource> GetConnectedResources(Part part, string resourceName)
        {
            List<PartResource> list = new List<PartResource>();
            PartResourceDefinition resDef = PartResourceLibrary.Instance.GetDefinition(resourceName);
            part.GetConnectedResources(resDef.id, resDef.resourceFlowMode, list);
            return list;
        }
        private ExperimentSituations GetScienceSituation(Vessel vessel)
        {
            return GetScienceSituation(vessel.altitude, vessel.situation, vessel.mainBody);
        }
        public ExperimentSituations GetScienceSituation(double altitude, Vessel.Situations situation, CelestialBody body)
        {
            CelestialBodyScienceParams scienceValues = body.scienceValues;
            ScreenMessages.PostScreenMessage(string.Format("{0} flyingAltitudeThreshold : {1}", body.name, scienceValues.flyingAltitudeThreshold), 3f, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(string.Format("{0} spaceAltitudeThreshold : {1}", body.name, scienceValues.spaceAltitudeThreshold), 3f, ScreenMessageStyle.UPPER_CENTER);
            ScreenMessages.PostScreenMessage(string.Format("{0} atmosphereScaleHeight : {1}", body.name, body.atmosphereScaleHeight), 3f, ScreenMessageStyle.UPPER_CENTER);
            ExperimentSituations result;
            if (situation == Vessel.Situations.LANDED)
            {
                result = ExperimentSituations.SrfLanded;
            }
            else
            {
                if (situation == Vessel.Situations.SPLASHED)
                {
                    result = ExperimentSituations.SrfSplashed;
                }
                else
                {
                    if (altitude <= (double)scienceValues.flyingAltitudeThreshold)
                    {
                        result = ExperimentSituations.FlyingLow;
                    }
                    else
                    {
                        if (altitude < Math.Max((double)body.maxAtmosphereAltitude, body.atmosphereScaleHeight * 1000.0 * 13.8))
                        {
                            result = ExperimentSituations.FlyingHigh;
                        }
                        else
                        {
                            if (altitude <= (double)scienceValues.spaceAltitudeThreshold)
                            {
                                result = ExperimentSituations.InSpaceLow;
                            }
                            else
                            {
                                result = ExperimentSituations.InSpaceHigh;
                            }
                        }
                    }
                }
            }
            return result;
        }
        public bool ConfirmLocation(ExperimentSituations experimentlocation)
        {
            bool result;
            if (experimentlocation == ExperimentSituations.SrfLanded)
            {
                string text = "SrfLanded";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == text || TargetSituation2 == text || TargetSituation3 == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation1 != text && TargetSituation2 != text && TargetSituation3 != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.SrfSplashed)
            {
                string text = "SrfSplashed";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == text || TargetSituation2 == text || TargetSituation3 == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation1 != text && TargetSituation2 != text && TargetSituation3 != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.FlyingLow)
            {
                string text = "FlyingLow";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == text || TargetSituation2 == text || TargetSituation3 == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation1 != text && TargetSituation2 != text && TargetSituation3 != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.FlyingHigh)
            {
                string text = "FlyingHigh";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == text || TargetSituation2 == text || TargetSituation3 == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation1 != text && TargetSituation2 != text && TargetSituation3 != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.InSpaceLow)
            {
                string text = "InSpaceLow";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == text || TargetSituation2 == text || TargetSituation3 == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation1 != text && TargetSituation2 != text && TargetSituation3 != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.InSpaceHigh)
            {
                string text = "InSpaceHigh";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation1 == text || TargetSituation2 == text || TargetSituation3 == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation1 != text && TargetSituation2 != text && TargetSituation3 != text)
                {
                    result = false;
                    return result;
                }
            }
            result = false;
            return result;
        }
        public override void OnFixedUpdate()
        {
            if (IsEnabled || AlwaysActive)
            {
                if (IsEnabled && !CheckElectricCharge())
                {
                    DeactivateConverter();
                    ScreenMessages.PostScreenMessage(string.Format("Warning! Insufficient Power to release the Chemical Trail!", new object[0]), 3f, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    if (IsEnabled && !CheckBody())
                    {
                        DeactivateConverter();
                        ScreenMessages.PostScreenMessage(string.Format("Warning! The Chemical Trail is only meant to be released while orbiting {0} only!", TargetBody), 3f, ScreenMessageStyle.UPPER_CENTER);
                    }
                    else
                    {
                        if (IsEnabled && !ConfirmLocation(GetScienceSituation(base.vessel)))
                        {
                            DeactivateConverter();
                            ScreenMessages.PostScreenMessage(string.Format("Warning! The Chemical Trail is only meant to be released in {0},{1},{2} situations only!", TargetSituation1, TargetSituation2, TargetSituation3), 3f, ScreenMessageStyle.UPPER_CENTER);
                        }
                        else
                        {
                            base.part.RequestResource(InputResource, InputRatio * TimeWarp.fixedDeltaTime);
                            base.part.RequestResource(OutputResource, OutputRatio * TimeWarp.fixedDeltaTime);
                        }
                    }
                }
            }
        }
        public override void OnStart(PartModule.StartState state)
        {
            base.Actions["ActivateConverterAction"].guiName = (base.Events["ActivateConverter"].guiName = string.Format("Release Chemical", new object[0]));
            base.Actions["DeactivateConverterAction"].guiName = (base.Events["DeactivateConverter"].guiName = string.Format("Deactivate Chemical", new object[0]));
            base.Actions["ToggleConverterAction"].guiName = string.Format("Toggle Data Recorder", new object[0]);
            base.Events["ActivateConverter"].guiActive = (base.Events["DeactivateConverter"].guiActive = !AlwaysActive);
            if (state != PartModule.StartState.Editor)
            {
                base.part.force_activate();
            }
        }
        public override void OnUpdate()
        {
            base.Events["ActivateConverter"].active = !IsEnabled;
            base.Events["DeactivateConverter"].active = IsEnabled;
        }
    }
}