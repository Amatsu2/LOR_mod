using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using RedFrogBufUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using カード効果;

namespace 専用ページ
{
    public class PostfixPatch_SetXmlInfo
    {
        public static void Postfix(BookXmlInfo classInfo, List<DiceCardXmlInfo> ____onlyCards)
        {
            if (classInfo.id.packageId == "PID")
            {
                ____onlyCards.Clear();

                foreach (int id in classInfo.EquipEffect.OnlyCard)
                {
                    LorId lid = new LorId("PID", id);

                    DiceCardXmlInfo info = ItemXmlDataList.instance.GetCardItem(lid);

                    if (info != null)
                    {
                        ____onlyCards.Add(info);
                    }
                }
            }
        }
    }
    public class test_Only : ModInitializer///"???_Only"
    {

        public static string path
        {
            get
            {
                bool flag = test_Only._path == null;///"???_Only"
                bool flag2 = flag;
                bool flag3 = flag2;
                bool flag4 = flag3;
                if (flag4)
                {
                    test_Only._path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));///"???_Only"
                }
                return test_Only._path;///"???_Only"
            }
        }

        public override void OnInitializeMod()
        {
            Harmony harmony = new Harmony(test_Only.packageId);///"???_Only"
            harmony.CreateClassProcessor(typeof(test_Only.BookModel_SetXmlInfo)).Patch();///"???_Only"
            Dictionary<string, BattleEffectText> dictionary = typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as Dictionary<string, BattleEffectText>;
            dictionary["test_Only"] = new BattleEffectText///"誰か_Only"
            {
                ID = "test_Only",///"???_Only"
                Name = "test専用バトルページ",///"[ここに誰かを記入]専用バトルページ"
                Desc = "このページはtestのページにのみ装着可能。"///"このページは [ここに誰かを記入]のページにのみ装着可能"
            };
            Singleton<ModContentManager>.Instance.GetErrorLogs().RemoveAll((string errorLog) => errorLog.Contains(test_Only.packageId) && errorLog.Contains("The same assembly name already exists"));///"???_Only"
        }

        public static string packageId = "Gomonkai";///ここにMODのidを記入

        protected static string _path = null;

        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();

        [HarmonyPatch(typeof(BookModel), "SetXmlInfo")]
        public class BookModel_SetXmlInfo
        {
            public static void Postfix(BookModel __instance, BookXmlInfo ____classInfo, ref List<DiceCardXmlInfo> ____onlyCards)
            {
                bool flag = __instance.BookId.packageId == test_Only.packageId;///"???_Only"
                bool flag2 = flag;
                bool flag3 = flag2;
                bool flag4 = flag3;
                if (flag4)
                {
                    foreach (int id in ____classInfo.EquipEffect.OnlyCard)
                    {
                        ____onlyCards.Add(ItemXmlDataList.instance.GetCardItem(new LorId(test_Only.packageId, id), false));///"???_Only"
                    }
                }
            }
        }
    }
}

namespace ダイス効果
//public class DiceCardAbility_tsu00D1"ダイス効果名" : DiceCardAbilityBase
{
}

namespace パッシブ
//public class PassiveAbility_tsu00P数"パッシブ名" : PassiveAbilityBase
{
    public class PassiveAbility_tsu00P1Gomonsword : PassiveAbilityBase
    //斬撃威力+1。攻撃が的中した時、火傷1または出血1を付与。
    {

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Slash)
            {
                owner.ShowPassiveTypo(this);
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1
                });
            }
        }
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (RandomUtil.valueForProb < 0.5)
            {
                owner.battleCardResultLog?.SetPassiveAbility(this);
                behavior.card.target?.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Burn, 1, owner);
            }
            else
            {
                owner.battleCardResultLog?.SetPassiveAbility(this);
                behavior.card.target?.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Bleeding, 1, owner);
            }
        }
    }

    public class PassiveAbility_tsu00P2Gomonlance : PassiveAbilityBase
    //貫通威力+1。貫通ダメージ量+2
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Penetrate)
            {
                owner.ShowPassiveTypo(this);
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1,
                    dmg = 2
                });
            }
        }
    }

    public class PassiveAbility_tsu00P3Gomongun : PassiveAbilityBase
    //遠距離ダイスの威力+1。遠距離ダイスの混乱ダメージ+2
    {
        public class DiceCardAbility_Gomongun : DiceCardAbilityBase
        {
            private PassiveAbility_tsu00P3Gomongun _passive;

            public DiceCardAbility_Gomongun(PassiveAbility_tsu00P3Gomongun pgun)
            {
                _passive = pgun;
            }

            public override void BeforeRollDice()
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1,
                    breakDmg = 2
                });
                base.owner.battleCardResultLog?.SetPassiveAbility(_passive);
            }
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetSpec().Ranged == CardRange.Far)
            {
                curCard.ApplyDiceAbility(DiceMatch.AllDice, new DiceCardAbility_Gomongun(this));
            }
        }
    }

    public class PassiveAbility_tsu00P4Gomonarrow : PassiveAbilityBase
    //遠距離ダイスの威力+1。遠距離ダイスのダメージ量+1
    {
        public class DiceCardAbility_Gomonarrow : DiceCardAbilityBase
        {
            private PassiveAbility_tsu00P4Gomonarrow _passive;

            public DiceCardAbility_Gomonarrow(PassiveAbility_tsu00P4Gomonarrow parrow)
            {
                _passive = parrow;
            }

            public override void BeforeRollDice()
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1,
                    dmg = 2
                });
                base.owner.battleCardResultLog?.SetPassiveAbility(_passive);
            }
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetSpec().Ranged == CardRange.Far)
            {
                curCard.ApplyDiceAbility(DiceMatch.AllDice, new DiceCardAbility_Gomonarrow(this));
            }

        }
    }

    public class PassiveAbility_tsu00P5Gomonglove : PassiveAbilityBase
    //打撃威力+1。打撃混乱ダメージ量+2
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail == BehaviourDetail.Hit)
            {
                owner.ShowPassiveTypo(this);
                behavior.ApplyDiceStatBonus(new DiceStatBonus
                {
                    power = 1,
                    breakDmg = 2
                });
            }
        }
    }

    public class PassiveAbility_tsu00P6BreakunitandEGO : PassiveAbilityBase
    //幕の開始時、虚弱と武装解除を5づつ得る。(虚弱の値+1)パワー、(武装解除の値+1)忍耐を得る。
    {
        public override void OnRoundStart()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Weak, 5, owner);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Disarm, 5, owner);

            int weakstack = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Weak);
            int disarmstack = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Disarm);


            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, weakstack + 1);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Endurance, disarmstack + 1);
        }
    }

    public class BattleUnitBuf_counterDicegard : BattleUnitBuf
    {
        protected override string keywordId => "counterDicegard";

        public override void OnAfterRollSpeedDice()
        {
            DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId("Gomonkai", 10));
            new DiceBehaviour();
            List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();
            int num = 0;
            foreach (DiceBehaviour diceBehaviour in cardItem.DiceBehaviourList)
            {
                BattleDiceBehavior battleDiceBehavior = new BattleDiceBehavior();
                battleDiceBehavior.behaviourInCard = diceBehaviour.Copy();
                battleDiceBehavior.behaviourInCard.Min = 3;
                battleDiceBehavior.behaviourInCard.Dice = 5;
                battleDiceBehavior.SetIndex(num++);
                list.Add(item: battleDiceBehavior);
            }
            _owner.cardSlotDetail.keepCard.AddBehaviours(cardItem, list);
        }
        public override void OnRoundEnd()
        {
            Destroy();
        }
    }

    public class PassiveAbility_tsu00P7CounterStrike : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(owner.faction);
            BattleUnitModel target = RandomUtil.SelectOne(aliveList);
            if (!target.bufListDetail.HasBuf<BattleUnitBuf_counterDicegard>())
            {
                target.bufListDetail.AddBuf(new BattleUnitBuf_counterDicegard());
            }
        }
    }


    public class PassiveAbility_tsu00P8gogyou : PassiveAbilityBase
    // 五行ページを追加する。
    {
        private int _sui = 50001;
        private int _kin = 50002;
        private int _moku = 50003;
        private int _ka = 50004;
        private int _do = 50005;
        private List<int> _onlyenemy = new List<int>();

        public override void OnRoundStart()
        {
            if (owner.faction == Faction.Player)
            {
                List<BattleDiceCardModel> hand = owner.personalEgoDetail.GetHand();
                bool flag = hand.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("Gomonkai", _sui));
                bool flag2 = hand.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("Gomonkai", _kin));
                bool flag3 = hand.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("Gomonkai", _moku));
                bool flag4 = hand.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("Gomonkai", _ka));
                bool flag5 = hand.Exists((BattleDiceCardModel x) => x.GetID() == new LorId("Gomonkai", _do));
                if (!flag && !flag2 && !flag3 && !flag4 && !flag5)
                {
                    owner.personalEgoDetail.AddCard(new LorId("Gomonkai", 50001));
                    owner.personalEgoDetail.AddCard(new LorId("Gomonkai", 50002));
                    owner.personalEgoDetail.AddCard(new LorId("Gomonkai", 50003));
                    owner.personalEgoDetail.AddCard(new LorId("Gomonkai", 50004));
                    owner.personalEgoDetail.AddCard(new LorId("Gomonkai", 50005));
                }
            }
        }

        public override void OnRoundStartAfter()
        {
            if (owner.faction == Faction.Enemy && owner.RollSpeedDice().FindAll((SpeedDice x) => !x.breaked).Count > 0 && !owner.IsBreakLifeZero())
            {
                bool flag = _onlyenemy.Exists((int x) => x == _sui);
                bool flag2 = _onlyenemy.Exists((int x) => x == _kin);
                bool flag3 = _onlyenemy.Exists((int x) => x == _moku);
                bool flag4 = _onlyenemy.Exists((int x) => x == _ka);
                bool flag5 = _onlyenemy.Exists((int x) => x == _do);
                if (!flag && !flag2 && !flag3 && !flag4 && !flag5)
                {
                    _onlyenemy.Add(_sui);
                    _onlyenemy.Add(_kin);
                    _onlyenemy.Add(_moku);
                    _onlyenemy.Add(_ka);
                    _onlyenemy.Add(_do);
                }
                int gogyou = RandomUtil.SelectOne(_onlyenemy);
                if (gogyou == _sui)
                {
                    DiceCardSelfAbility_tsu00C1gogyousui.Activate(owner);
                }
                if (gogyou == _kin)
                {
                    DiceCardSelfAbility_tsu00C2gogyoukin.Activate(owner);
                }
                if (gogyou == _moku)
                {
                    DiceCardSelfAbility_tsu00C3gogyoumoku.Activate(owner);
                }
                if (gogyou == _ka)
                {
                    DiceCardSelfAbility_tsu00C4gogyouka.Activate(owner);
                }
                if (gogyou == _do)
                {
                    DiceCardSelfAbility_tsu00C5gogyoudo.Activate(owner);
                }
                _onlyenemy.Remove(gogyou);
            }
        }
    }
    public class PassiveAbility_tsu00P9kioku : PassiveAbilityBase
    //「追憶」バフを得る(最大15)。追憶ページでマッチに敗北すると「追憶」を1失う。「追憶」のスタックに応じでバフを得る。記憶が0になると全ての耐性が脆弱になる
    {

    }
}


namespace カード効果
//public class DiceCardSelfAbility_tsu00C1"カード効果名" : DiceCardSelfAbilityBase

{
    public class BattleUnitBuf_tsu00BU1addsinking : BattleUnitBuf
    //的中時に沈潜1付与
    {
        protected override string keywordId => "Gogyou_sui";
        public override void OnSuccessAttack(BattleDiceBehavior behavior)
        {
            behavior.card.target?.bufListDetail.AddKeywordBufByEtc(RedFrogKeywordBuf.R_Sinking, 1, _owner);
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }
    }
    public class BattleUnitBuf_tsu00BU2nodmg : BattleUnitBuf
    //ダメージを与えない,ダイスの本来の値÷2回復(小数点以下切り捨て
    {
        protected override string keywordId => "Gogyou_kin";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmgRate = -9999,
                breakRate = -9999
            });
        }
        public override void OnSuccessAttack(BattleDiceBehavior behavior)
        {
            int healamount = behavior.DiceVanillaValue / 2;
            if (healamount > 0)
            {
                _owner.RecoverHP(healamount);
                healamount = 0;
            }
        }
    }

    public class BattleUnitBuf_tsu00BU4ignorepower : BattleUnitBuf
    //互いに威力の効果を受けない
    {
        protected override string keywordId => "Gogyou_do";
        public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
        {
            card.ignorePower = true;
        }
        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            BattleUnitModel target = card.target;
            if (target != null && target.currentDiceAction != null)
            {
                target.currentDiceAction.ignorePower = true;
            }
        }
    }

    public class BattleUnitBuf_gogyouCommon : BattleUnitBuf
    {
        public BattleUnitBuf_gogyouCommon()
        {
            stack = 0;
        }
        public override void OnRoundEnd()
        {
            Destroy();
        }
    }

    public class DiceCardSelfAbility_tsu00gogyouCommon : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return !owner.bufListDetail.HasBuf<BattleUnitBuf_gogyouCommon>();
        }
    }

    public class DiceCardSelfAbility_tsu00C1gogyousui : DiceCardSelfAbility_tsu00gogyouCommon
    {
        public static string Desc = "五行ページは1幕に1回のみ使用可能。まだ使用していない他の五行ページを全て使用した後に再度使用可能" +
            "[装着時発動]この幕の間パワー1を得る。攻撃的中時に沈潜1を付与";
        public override string[] Keywords => new string[2] { "Strength_Keyword", "RedFrog_Sinking" };
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            DiceCardSelfAbility_tsu00C1gogyousui.Activate(unit);
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            self.exhaust = true;
        }
        public static void Activate(BattleUnitModel unit)
        {
            unit.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Strength, 1, unit);
            unit.bufListDetail.AddBuf(new BattleUnitBuf_tsu00BU1addsinking());
            unit.bufListDetail.AddBuf(new BattleUnitBuf_gogyouCommon());
        }
    }

    public class DiceCardSelfAbility_tsu00C2gogyoukin : DiceCardSelfAbility_tsu00gogyouCommon
    {
        public static string Desc = "五行ページは1幕に1回のみ使用可能。まだ使用していない他の五行ページを全て使用した後に再度使用可能" +
            "[装着時発動]この幕に保護1を得る。攻撃ダイスはダメージを与えることが出来ない。" +
            "代わりに、バフを差し引いたダイスの値の半分だけ体力を回復することが出来る。(小数点以下切り捨て)";
        public override string[] Keywords => new string[1] { "Protection" };
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            DiceCardSelfAbility_tsu00C2gogyoukin.Activate(unit);
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            self.exhaust = true;
        }
        public static void Activate(BattleUnitModel unit)
        {
            unit.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Protection, 1, unit);
            unit.bufListDetail.AddBuf(new BattleUnitBuf_tsu00BU2nodmg());
            unit.bufListDetail.AddBuf(new BattleUnitBuf_gogyouCommon());
        }
    }

    public class DiceCardSelfAbility_tsu00C3gogyoumoku : DiceCardSelfAbility_tsu00gogyouCommon
    {
        public static string Desc = "五行ページは1幕に1回のみ使用可能。まだ使用していない他の五行ページを全て使用した後に再度使用可能" +
            "[装着時発動]この幕に";
        public override string[] Keywords => new string[] { "" };
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            DiceCardSelfAbility_tsu00C3gogyoumoku.Activate(unit);
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            self.exhaust = true;
        }
        public static void Activate(BattleUnitModel unit)
        {
            unit.bufListDetail.AddBuf(new BattleUnitBuf_gogyouCommon());
        }
    }

    public class DiceCardSelfAbility_tsu00C4gogyouka : DiceCardSelfAbility_tsu00gogyouCommon
    {
        public static string Desc = "五行ページは1幕に1回のみ使用可能。まだ使用していない他の五行ページを全て使用した後に再度使用可能" +
            "[装着時発動]この幕に";
        public override string[] Keywords => new string[] { "" };
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            DiceCardSelfAbility_tsu00C4gogyouka.Activate(unit);
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            self.exhaust = true;
        }
        public static void Activate(BattleUnitModel unit)
        {
            unit.bufListDetail.AddBuf(new BattleUnitBuf_gogyouCommon());
        }
    }
    public class DiceCardSelfAbility_tsu00C5gogyoudo : DiceCardSelfAbility_tsu00gogyouCommon
    {
        public static string Desc = "五行ページは1幕に1回のみ使用可能。まだ使用していない他の五行ページを全て使用した後に再度使用可能" +
            "[装着時発動]この幕の間このページを装着したキャラクターと、このキャラクターとマッチしたした相手は互いに威力の効果を受けない。";
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            DiceCardSelfAbility_tsu00C5gogyoudo.Activate(unit);
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
            self.exhaust = true;
        }
        public static void Activate(BattleUnitModel unit)
        {
            unit.bufListDetail.AddBuf(new BattleUnitBuf_tsu00BU4ignorepower());
            unit.bufListDetail.AddBuf(new BattleUnitBuf_gogyouCommon());

        }
    }
    public class DiceCardSelfAbility_tsu00C6gogyouranbu : DiceCardSelfAbility_tsu00gogyouCommon
    {
        public static string Desc = "この幕に五行ページを使用した場合、このダイスを4回再使用";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {

        }
    }

}
