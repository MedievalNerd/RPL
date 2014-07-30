using System;

namespace CustomExperiment
{
    public class ChemicalReleaseExperiment : ModuleScienceExperiment
    {
        [KSPField(isPersistant = false)]
        public string ExperimentChemical;
        [KSPField(isPersistant = false)]
        public string TargetBody;
        [KSPField(isPersistant = false)]
        public string TargetSituation;
        private ExperimentSituations GetScienceSituation(Vessel vessel)
        {
            return GetScienceSituation(vessel.altitude, vessel.situation, vessel.mainBody);
        }
        public ExperimentSituations GetScienceSituation(double altitude, Vessel.Situations situation, CelestialBody body)
        {
            CelestialBodyScienceParams scienceValues = body.scienceValues;
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
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.SrfSplashed)
            {
                string text = "SrfSplashed";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.FlyingLow)
            {
                string text = "FlyingLow";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.FlyingHigh)
            {
                string text = "FlyingHigh";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.InSpaceLow)
            {
                string text = "InSpaceLow";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.InSpaceHigh)
            {
                string text = "InSpaceHigh";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            result = false;
            return result;
        }
        private bool CheckBody()
        {
            return base.vessel.mainBody.name == TargetBody;
        }
        private bool CheckChemical()
        {
            PartResource partResource = base.part.Resources.list.Find((PartResource pr) => pr.resourceName == ExperimentChemical);
            return !(partResource == null) && partResource.amount == 0.0;
        }
        private bool ValidateExperiment()
        {
            bool result;
            if (!CheckBody())
            {
                ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment is only calibrated for {0}!", TargetBody), 3f, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment is only calibrated for {0} situation!", TargetSituation), 3f, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment requires to have dispersed the entire canister of {0}!", ExperimentChemical), 3f, ScreenMessageStyle.UPPER_CENTER);
                result = false;
            }
            else
            {
                if (!ConfirmLocation(GetScienceSituation(base.vessel)))
                {
                    ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment is only calibrated for {0} situation!", TargetSituation), 3f, ScreenMessageStyle.UPPER_CENTER);
                    ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment requires to have dispersed the entire canister of {0}!", ExperimentChemical), 3f, ScreenMessageStyle.UPPER_CENTER);
                    result = false;
                }
                else
                {
                    if (!CheckChemical())
                    {
                        ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment requires to have dispersed the entire canister of {0}!", ExperimentChemical), 3f, ScreenMessageStyle.UPPER_CENTER);
                        result = false;
                    }
                    else
                    {
                        if (ConfirmLocation(GetScienceSituation(base.vessel)) && CheckBody() && CheckChemical())
                        {
                            ScreenMessages.PostScreenMessage(string.Format("Success! We've completed the requirements to complete this experiment!", new object[0]), 3f, ScreenMessageStyle.UPPER_CENTER);
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }
        public new void DeployAction(KSPActionParam p)
        {
            if (ValidateExperiment())
            {
                base.DeployAction(p);
            }
        }
        public new void DeployExperiment()
        {
            if (ValidateExperiment())
            {
                base.DeployExperiment();
            }
        }
    }
}