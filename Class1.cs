using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static DiceCardSelfAbility_hanaDiscardDraw;
using static カード.DiceCardSelfAbility_tsu00UniteC4NextBreakProtection;
using static カード.DiceCardSelfAbility_tsu00UniteC5Preparation;

namespace ダイス効果
{
    public class DiceCardAbility_tsu00D1randomprptection : DiceCardAbilityBase
    {
        public static string Desc = "[的中時]ランダムな味方1名に今回の幕に保護1を付与";
        public override string[] Keywords => new string[1] { "Protection_Keyword" };
        public override void OnSucceedAttack()
        {
            foreach (BattleUnitModel item in BattleObjectManager.instance.GetAliveList_random(base.owner.faction, 2))
            {
                item.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Protection, 1, base.owner);
            }
        }
    }

    public class DiceCardAbility_tsu00D2randombreakprptection : DiceCardAbilityBase
    {
        public static string Desc = "[的中時]ランダムな味方1名に今回の幕に混乱保護1を付与";
        public override string[] Keywords => new string[1] { "BreakProtection_Keyword" };
        public override void OnSucceedAttack()
        {
            foreach (BattleUnitModel item in BattleObjectManager.instance.GetAliveList_random(base.owner.faction, 2))
            {
                item.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.BreakProtection, 1, base.owner);
            }
        }
    }
}

namespace パッシブ
{
    public class PassiveAbility_tsu00P1AllForUnite : PassiveAbilityBase
    //結束ページを使用するとき、全ダイスの最小値+2。感情レベルが3以上なら追加で全ダイスの最大値+1。
    {
        public override void BeforeRollDice(BattleDiceBehavior curcard)
        {
            if (curcard.card.cardAbility != null && curcard.card.cardAbility.IsUniteCard)
            {
                int min = 2;
                int max = 0;

                if (this.owner.emotionDetail.EmotionLevel >= 3)
                {
                    max = 1;
                }
                curcard.ApplyDiceStatBonus(new DiceStatBonus
                {
                    min = min,
                    max = max
                });
            }
        }
    }


    public class PassiveAbility_tsu00P2UniteForAll : PassiveAbilityBase
    // 結束ページで自分にパワー、忍耐、保護、混乱保護が付与されるとき、他のランダムな味方1人に同じバフを付与
    {
        private bool _isUsingUniteCard = false;

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            _isUsingUniteCard = curCard.cardAbility != null && curCard.cardAbility.IsUniteCard;
            base.OnUseCard(curCard);
        }

        public override void OnAddKeywordBufByCardForEvent(KeywordBuf keywordBuf, int stack, BufReadyType readyType)
        {
            if (!_isUsingUniteCard) return;

            if (keywordBuf != KeywordBuf.Strength &&
                keywordBuf != KeywordBuf.Endurance &&
                keywordBuf != KeywordBuf.Protection &&
                keywordBuf != KeywordBuf.BreakProtection)
            {
                return;
            }

            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(owner.faction);
            aliveList.Remove(owner);

            for (int i = 0; i < 1 && aliveList.Count > 0; i++)
            {
                BattleUnitModel target = RandomUtil.SelectOne(aliveList);

                switch (readyType)
                {
                    case BufReadyType.ThisRound:
                        target.bufListDetail.AddKeywordBufByEtc(keywordBuf, stack);
                        owner.UnitData.historyInWave.ch7oneforall++;
                        break;
                    case BufReadyType.NextRound:
                        target.bufListDetail.AddKeywordBufByEtc(keywordBuf, stack);
                        owner.UnitData.historyInWave.ch7oneforall++;
                        break;
                    case BufReadyType.NextNextRound:
                        Debug.LogError("invalid ready Type");
                        break;
                }
            }
        }
    }

    public class PassiveAbility_tsu00P3UniqueCountpower : PassiveAbilityBase
    //1幕の間結束ページを使用した枚数に応じて次の幕にバフを得る。2枚:自身にパワー1付与　3枚:自身にパワー2、忍耐1付与　4枚以上:味方全体にパワー2、忍耐2付与
    {
        private int _uniteCardUsedCount = 0;

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.cardAbility != null && curCard.cardAbility.IsUniteCard)
            {
                _uniteCardUsedCount++;
            }
        }

        public override void OnRoundEnd()
        {
            if (_uniteCardUsedCount == 2)
            {
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1);
            }
            else if (_uniteCardUsedCount == 3)
            {
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 2);
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1);
            }
            else if (_uniteCardUsedCount >= 4)
            {
                foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList(owner.faction))
                {
                    alive.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 2);
                    alive.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 2);
                }
            }
            _uniteCardUsedCount = 0;
        }
    }


    public class PassiveAbility_tsu00P4UniqueCountprotection : PassiveAbilityBase
    //1幕の間結束ページを使用した枚数に応じて次の幕にバフを得る。2枚:自身に保護1付与　3枚:自身に保護2、混乱保護1付与　4枚以上:味方全体に保護2、混乱保護2付与
    {
        private int _uniteCardUsedCount = 0;

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.cardAbility != null && curCard.cardAbility.IsUniteCard)
            {
                _uniteCardUsedCount++;
            }
        }
        public override void OnRoundEnd()
        {
            if (_uniteCardUsedCount == 2)
            {
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 1);
            }
            else if (_uniteCardUsedCount == 3)
            {
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 2);
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 1);
            }
            else if (_uniteCardUsedCount >= 4)
            {
                foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList(owner.faction))
                {
                    alive.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 2);
                    alive.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 2);
                }
            }
            _uniteCardUsedCount = 0;
        }
    }

    public class PassiveAbility_tsu00P4UniteForAllextend : PassiveAbilityBase
    // 結束ページで自分にパワー、忍耐、保護、混乱保護が付与されるとき、他のランダムな味方2人に同じバフを付与
    {
        private bool _isUsingUniteCard = false;

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            _isUsingUniteCard = curCard.cardAbility != null && curCard.cardAbility.IsUniteCard;
            base.OnUseCard(curCard);
        }

        public override void OnAddKeywordBufByCardForEvent(KeywordBuf keywordBuf, int stack, BufReadyType readyType)
        {
            if (!_isUsingUniteCard) return;

            if (keywordBuf != KeywordBuf.Strength &&
                keywordBuf != KeywordBuf.Endurance &&
                keywordBuf != KeywordBuf.Protection &&
                keywordBuf != KeywordBuf.BreakProtection)
            {
                return;
            }

            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(owner.faction);
            aliveList.Remove(owner);

            for (int i = 0; i < 2 && aliveList.Count > 0; i++)
            {
                BattleUnitModel target = RandomUtil.SelectOne(aliveList);

                switch (readyType)
                {
                    case BufReadyType.ThisRound:
                        target.bufListDetail.AddKeywordBufByEtc(keywordBuf, stack);
                        owner.UnitData.historyInWave.ch7oneforall++;
                        break;
                    case BufReadyType.NextRound:
                        target.bufListDetail.AddKeywordBufByEtc(keywordBuf, stack);
                        owner.UnitData.historyInWave.ch7oneforall++;
                        break;
                    case BufReadyType.NextNextRound:
                        Debug.LogError("invalid ready Type");
                        break;
                }
            }
        }
    }

}

namespace カード
{
    public class DiceCardSelfAbility_tsu00UniteC1NextPower : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全体に「結束ページを使用時、次の幕にパワー1を得る(重複しない)」を追加」";
        public override string[] Keywords => new string[1] { "Strength_Keyword" };
        public override bool IsUniteCard => true;

        public class BattleunitBuf_C1NextPower : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    this._owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, base._owner);
                }
            }
            public override void OnRoundEnd()
            {
                this.Destroy();
            }

        }
        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in
                BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleunitBuf_C1NextPower>())
                {
                    alive.bufListDetail.AddBuf(new BattleunitBuf_C1NextPower());

                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC2NextEndurance : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全体に「結束ページを使用時、次の幕に忍耐1を得る(重複しない)」を付与";
        public override string[] Keywords => new string[1] { "Endurance_Keyword" };
        public override bool IsUniteCard => true;

        public class BattleunitBuf_C2NextEndurance : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    this._owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Endurance, 1, base._owner);
                }
            }
            public override void OnRoundEnd()
            {
                this.Destroy();
            }

        }
        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in
                BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleunitBuf_C2NextEndurance>())
                {
                    alive.bufListDetail.AddBuf(new BattleunitBuf_C2NextEndurance());

                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC3NextProtection : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全体に「結束ページを使用時、次の幕に保護1を得る(重複しない)」を付与";
        public override string[] Keywords => new string[1] { "Protection_Keyword" };
        public override bool IsUniteCard => true;

        public class BattleunitBuf_C3NextProtection : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    this._owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Protection, 1, base._owner);
                }
            }
            public override void OnRoundEnd()
            {
                this.Destroy();
            }

        }
        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in
                BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleunitBuf_C3NextProtection>())
                {
                    alive.bufListDetail.AddBuf(new BattleunitBuf_C3NextProtection());

                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC4NextBreakProtection : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全体に「結束ページを使用時、次の幕に混乱保護1を得る(重複しない)」を付与";
        public override string[] Keywords => new string[1] { "BreakProtection_Keyword" };
        public override bool IsUniteCard => true;

        public class BattleunitBuf_C4NextBreakProtection : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    this._owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.BreakProtection, 1, base._owner);
                }
            }
            public override void OnRoundEnd()
            {
                this.Destroy();
            }

        }
        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in
                BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleunitBuf_C4NextBreakProtection>())
                {
                    alive.bufListDetail.AddBuf(new BattleunitBuf_C4NextBreakProtection());

                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC5Preparation : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全体に「結束ページを使用時、そのページに反撃ダイス(3-7)を追加(重複しない)」を付与";
        public class BattleUnitBuf_C5Preparation : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility == null || !card.cardAbility.IsUniteCard)
                {
                    return;
                }
                new BattleDiceBehavior();
                BattleDiceCardModel battleDiceCardModel = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId("Yui_Workshop", 8), false));
                if (battleDiceCardModel == null)
                {
                    return;
                }
                foreach (BattleDiceBehavior item in battleDiceCardModel.CreateDiceCardBehaviorList())
                {
                    card.AddDice(item);
                }
            }

            public override void OnRoundEnd()
            {
                Destroy();
            }
        }

        public override bool IsUniteCard => true;

        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleUnitBuf_C5Preparation>())
                {
                    alive.bufListDetail.AddBuf(new BattleUnitBuf_C5Preparation());
                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC6DiscardDraw : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全員に「結束ページを使用時、ページをランダムに1枚捨て、次の幕の開始時にページを2枚引く(重複しない)」を付与」";
        public override string[] Keywords => new string[1] { "DrawCard_Keyword" };
        public override bool IsUniteCard => true;

        public class BattleunitBuf_C6DiscardDraw : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    base._owner.allyCardDetail.DiscardACardRandomlyByAbility(1);
                }
            }
            public override void OnRoundEnd()
            {
                _owner.allyCardDetail.DrawCards(2);
                Destroy();
            }
        }

        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in
                BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleunitBuf_C6DiscardDraw>())
                {
                    alive.bufListDetail.AddBuf(new BattleunitBuf_C6DiscardDraw());
                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC7highlander : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全員に「結束ページを使用時、唯一状態なら全ダイス威力+2(重複しない)」を付与」";
        public override string[] Keywords => new string[1] { "OnlyOne_Keyword" };
        public override bool IsUniteCard => true;
        public class BattleunitBuf_C7highlander : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard && base._owner.allyCardDetail.IsHighlander())
                {
                    card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                    {
                        power = 2
                    });
                }
            }

        }
        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in
                BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleunitBuf_C7highlander>())
                {
                    alive.bufListDetail.AddBuf(new BattleunitBuf_C7highlander());
                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC9HealLightdraw : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[戦闘開始時]今回の幕の間、他の味方全員に「結束ページを使用時、光1回復、ページ1枚を引く(重複しない)」を付与";
        public override string[] Keywords => new string[2] { "Energy_Keyword", "DrawCard_Keyword" };

        public override bool IsUniteCard => true;

        public class BattleunitBuf_C8HealLight : BattleUnitBuf
        {
            public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
            {
                if (card.cardAbility != null && card.cardAbility.IsUniteCard)
                {
                    base._owner.cardSlotDetail.RecoverPlayPointByCard(1);
                    base._owner.allyCardDetail.DrawCards(1);
                }
            }
            public override void OnRoundEnd()
            {
                Destroy();
            }
        }

        public override void OnStartBattle()
        {
            foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList(base.owner.faction))
            {
                if (alive != base.owner && !alive.bufListDetail.HasBuf<BattleunitBuf_C8HealLight>())
                {
                    alive.bufListDetail.AddBuf(new BattleunitBuf_C8HealLight());
                }
            }
        }
    }

    public class DiceCardSelfAbility_tsu00UniteC10heallight : DiceCardSelfAbilityBase
    {
        public static string Desc = "[結束ページ]" +
            "[使用時]光1回復";
        public override string[] Keywords => new string[1] { "Energy_Keyword" };

        public override bool IsUniteCard => true;
        public override void OnUseCard()
        {
            base.owner.cardSlotDetail.RecoverPlayPointByCard(1);
        }
    }
}