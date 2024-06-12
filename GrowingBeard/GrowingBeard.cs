using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Library;

namespace GrowingBeard
{
    class GrowingBeard : CampaignBehaviorBase
    {
        int _beardStage = 0;
        int _maxBeardStage = 9;
        bool _hasStartedMod = false;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
        }
        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            obj.AddPlayerLine("tavernkeeper_talk_to_start_beardmod", "tavernkeeper_talk", "tavernkeeper_pretalk", "{=MikkoK_GB_001}I would like to start growing out my beard.", new ConversationSentence.OnConditionDelegate(conversation_tavernkeep_modstarted_already), new ConversationSentence.OnConsequenceDelegate(this.conversation_start_mod), 100, null, null);
            obj.AddPlayerLine("tavernkeeper_barber", "tavernkeeper_talk", "tavernkeeper_offer_barber", "{=MikkoK_GB_002}I would like to get my beard trimmed.", new ConversationSentence.OnConditionDelegate(this.conversation_tavernkeep_modstarted), null, 100, null, null);
            obj.AddDialogLine("tavernkeeper_barber_shop", "tavernkeeper_offer_barber", "tavernkeeper_beard_trimmer", "{=MikkoK_GB_003}That would be 20 denars, what length do you want your beard to be trimmed to?", null, null, 0, null);
            obj.AddPlayerLine("tavernkeeper_clean_shaven", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_004}Clean shaven.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(0)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(0)), 10, null, null);
            obj.AddPlayerLine("tavernkeeper_very_short_stubble", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_005}Slight stubble.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(1)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(1)), 20, null, null);
            obj.AddPlayerLine("tavernkeeper_stubble", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_006}Stubble.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(2)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(2)), 30, null, null);
            obj.AddPlayerLine("tavernkeeper_longer_stubble", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_007}Longer stubble.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(3)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(3)), 40, null, null);
            obj.AddPlayerLine("tavernkeeper_very_short_beard", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_008}Short beard.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(4)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(4)), 50, null, null);
            obj.AddPlayerLine("tavernkeeper_beard", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_009}Beard.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(5)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(5)), 60, null, null);
            obj.AddPlayerLine("tavernkeeper_full_beard", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_010}Full beard.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(6)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(6)), 70, null, null);
            obj.AddPlayerLine("tavernkeeper_massive_beard", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_011}Big bush.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(7)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(7)), 80, null, null);
            obj.AddPlayerLine("tavernkeeper_huge_beard", "tavernkeeper_beard_trimmer", "tavernkeeper_shave", "{=MikkoK_GB_012}Huge bush.", new ConversationSentence.OnConditionDelegate(() => this.conversation_tavernkeep_check_beard_length(8)), new ConversationSentence.OnConsequenceDelegate(() => this.conversation_tavernkeep_shave_beard(8)), 90, null, null);
            obj.AddPlayerLine("tavernkeeper_deny_shave", "tavernkeeper_beard_trimmer", "tavernkeeper_pretalk", "{=MikkoK_GB_013}Nevermind.", null, null, 100, null, null);
            obj.AddPlayerLine("tavernkeeper_talk_to_stop_beardmod", "tavernkeeper_talk", "tavernkeeper_pretalk", "{=MikkoK_GB_014}I would like to stop growing out my beard.", new ConversationSentence.OnConditionDelegate(conversation_tavernkeep_modstarted), new ConversationSentence.OnConsequenceDelegate(this.conversation_stop_mod), 100, null, null);
            obj.AddDialogLine("tavernkeeper_barber_thank", "tavernkeeper_shave", "tavernkeeper_pretalk", "{=MikkoK_GB_015}I'll give you a shave before you leave.", null, null, 100, null);
        }

        public void DailyTick()
        {
            if (_hasStartedMod && _beardStage < 3)
            {
                this.GrowBeard();
            }
        }
        public void WeeklyTick()
        {
            if (_hasStartedMod && _beardStage >= 3)
            {
                this.GrowBeard();
            }
        }
        private void conversation_tavernkeep_shave_beard(int beardValue)
        {
            ShaveBeard(beardValue, true);
            _beardStage = beardValue;
        }

        private bool conversation_tavernkeep_check_beard_length(int i)
        {
            if (_beardStage > i && Hero.MainHero.Gold > 10)
            {
                return true;
            }
            else
                return false;
        }

        private void conversation_start_mod()
        {
            if (!Hero.MainHero.IsFemale)
            {
                _hasStartedMod = true;
                _beardStage = 0;
                ShaveBeard(_beardStage, false);
                InformationManager.DisplayMessage(new InformationMessage("{=MikkoK_GB_016}Beard Growing mod started, beard has been set to shaven, please do not change it while running the mod."));
            }
        }

        private void conversation_stop_mod()
        {
            _hasStartedMod = false;
            InformationManager.DisplayMessage(new InformationMessage("{=MikkoK_GB_017}Beard Growing mod stopped, you can alter your current beard by pressing V on world map."));
        }

        private bool conversation_tavernkeep_modstarted()
        {
            return _hasStartedMod;
        }

        private bool conversation_tavernkeep_modstarted_already()
        {
            if (_hasStartedMod || Hero.MainHero.IsFemale)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void GrowBeard()
        {
            if (_beardStage < _maxBeardStage && _hasStartedMod)
            {
                _beardStage++;
                SetBeardState(_beardStage);
                InformationManager.DisplayMessage(new InformationMessage("{=MikkoK_GB_018}Your beard has grown."));
            }
            if (_beardStage == _maxBeardStage)
            {
                InformationManager.DisplayMessage(new InformationMessage("{=MikkoK_GB_019}Your beard has grown to maximum length."));
                return;
            }
        }


        private void SetBeardState(int beardState)
        {
            switch (beardState)
            {
                case 0:
                    ChangeBeardTo(0);
                    break;
                case 1:
                    ChangeBeardTo(25);
                    break;
                case 2:
                    ChangeBeardTo(26);
                    break;
                case 3:
                    ChangeBeardTo(23);
                    break;
                case 4:
                    ChangeBeardTo(13);
                    break;
                case 5:
                    ChangeBeardTo(14);
                    break;
                case 6:
                    ChangeBeardTo(15);
                    break;
                case 7:
                    ChangeBeardTo(16);
                    break;
                case 8:
                    ChangeBeardTo(17);
                    break;
                case 9:
                    ChangeBeardTo(18);
                    break;
                default:
                    break;
            }
        }

        private void ShaveBeard(int beard, bool announce)
        {
            SetBeardState(beard);
            if (announce)
            {
                InformationManager.DisplayMessage(new InformationMessage("{=MikkoK_GB_020}Your beard has been shaved."));
            }
        }
        private void ChangeBeardTo(int beard)
        {
            Hero.MainHero.ModifyHair(-1, beard, -1);
        }


        public override void SyncData(IDataStore dataStore)
        {
            try
            {

                dataStore.SyncData("_hasStartedMod", ref this._hasStartedMod);
                dataStore.SyncData("_beardStage", ref this._beardStage);
            }
            catch (NullReferenceException doesntExist)
            {

            }
        }


    }
}
