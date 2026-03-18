using System;

namespace ProofAcidFireCold.Constants
{
    /// <summary>
    /// 游戏相关常量的集中定义
    /// </summary>
    public static class GameConstants
    {
        /// <summary>
        /// 元素ID常量（来源于游戏Element系统）
        /// </summary>
        public static class ElementIds
        {
            /// <summary>火元素ID</summary>
            public const int Fire = 910;

            /// <summary>冰冻元素ID</summary>
            public const int Cold = 911;

            /// <summary>酸性元素ID</summary>
            public const int Acid = 923;
        }

        /// <summary>
        /// 物品分类ID常量（来源于游戏Category系统）
        /// </summary>
        public static class CategoryIds
        {
            /// <summary>食材分类</summary>
            public const string Foodstuff = "foodstuff";

            /// <summary>垃圾分类</summary>
            public const string Garbage = "garbage";

            /// <summary>废品分类</summary>
            public const string Junk = "junk";
        }

        /// <summary>
        /// 游戏消息键常量（用于显示本地化消息）
        /// </summary>
        public static class MessageKeys
        {
            /// <summary>物品栏中的毯子保护消息前缀</summary>
            public const string BlanketInventory = "blanketInv_";

            /// <summary>地面上的毯子保护消息前缀</summary>
            public const string BlanketGround = "blanketGround_";

            /// <summary>阻止偷钱的消息键</summary>
            public const string StealNegateMoney = "abStealNegateMoney";

            /// <summary>阻止偷窃的消息键</summary>
            public const string StealNegate = "abStealNegate";
        }

        /// <summary>
        /// 行为引用键常量（用于ActRef.n1等字段）
        /// </summary>
        public static class ActRefKeys
        {
            /// <summary>金钱标识</summary>
            public const string Money = "money";
        }
    }
}
