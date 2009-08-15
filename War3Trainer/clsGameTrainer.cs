using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace War3Trainer
{
    #region Node type

    //////////////////////////////////////////////////////////////////////////
    // Function node
    enum TrainerNodeType
    {
        None,
        Root,

        Introduction,
        Cash,
        AllSelectedUnit,
        OneSelectedUnit,
        UnitPropety,
        HeroPropety,
        //UnitAbility,  // Not implemented yet
        AllUnitGoods,
        OneGoods,
    }

    internal class NewChildrenEventArgs
        : EventArgs, ICloneable
    {
        public TrainerNodeType NodeType { get; set; }
        public int ParentNodeIndex { get; set; }

        public UInt32 pThisGame { get; set; }           // [6FAA4178]
        public UInt32 pThisGameMemory { get; set; }     // [ThisGame + 0xC]
        public UInt32 pThisUnit { get; set; }           // Unit ESI
        public UInt32 pUnitAttributes { get; set; }     // [ThisUnit + 1E4]
        public UInt32 pHeroAttributes { get; set; }     // [ThisUnit + 1EC]
        public UInt32 pCurrentGoods { get; set; }       // [ThisUnit + 1F4]...

        public NewChildrenEventArgs()
        {
        }

        public object Clone()
        {
            NewChildrenEventArgs retObject = new NewChildrenEventArgs();
            retObject.NodeType = this.NodeType;
            retObject.ParentNodeIndex = this.ParentNodeIndex;
            retObject.pThisGame = this.pThisGame;
            retObject.pThisGameMemory = this.pThisGameMemory;
            retObject.pThisUnit = this.pThisUnit;
            retObject.pUnitAttributes = this.pUnitAttributes;
            retObject.pHeroAttributes = this.pHeroAttributes;
            retObject.pCurrentGoods = this.pCurrentGoods;
            return retObject;
        }
    }
    
    internal interface ITrainerNode
    {
        TrainerNodeType NodeType { get; }
        string NodeTypeName { get; }
        int NodeIndex { get; }
        int ParentIndex { get; }
    }
    
    //////////////////////////////////////////////////////////////////////////
    // Address list node
    internal enum AddressListValueType
    {
        Integer,
        Float,
        Char4
    }

    internal class NewAddressListEventArgs
        : EventArgs
    {
        public int ParentNodeIndex;
        public string Caption;

        public UInt32 Address;
        public AddressListValueType ValueType;
        public int ValueScale;

        public NewAddressListEventArgs(
            int ParentNodeIndex,
            string Caption,
            UInt32 Address,
            AddressListValueType ValueType)
            : this(ParentNodeIndex, Caption, Address, ValueType, 1)
        {
        }

        public NewAddressListEventArgs(
            int ParentNodeIndex,
            string Caption,
            UInt32 Address,
            AddressListValueType ValueType,
            int ValueScale)
        {
            this.ParentNodeIndex = ParentNodeIndex;
            this.Caption = Caption;

            this.Address = Address;
            this.ValueType = ValueType;
            this.ValueScale = ValueScale;
        }
    }

    internal interface IAddressNode
    {
        int ParentIndex { get; }

        string Caption { get; }
        UInt32 Address { get; }
        AddressListValueType ValueType { get; }
        int ValueScale { get; }
    }
    #endregion

    #region Basic nodes
    
    internal delegate void NewChildrenEventHandler(object sender, NewChildrenEventArgs e);
    internal delegate void NewAddressListEventHandler(object sender, NewAddressListEventArgs e);

    internal class TrainerNode
    {
        public NewChildrenEventHandler      NewChildren;
        public NewAddressListEventHandler   NewAddress;

        public virtual void CreateChildren()
        {
        }

        protected void CreateChild(
            TrainerNodeType ChildType,
            int ParentIndex)
        {
            if (NewChildren != null)
            {
                NewChildrenEventArgs Args = _NewChildrenArgs.Clone() as NewChildrenEventArgs;
                Args.NodeType = ChildType;
                Args.ParentNodeIndex = ParentIndex;
                NewChildren(this, Args);
            }
        }

        protected void CreateAddress(NewAddressListEventArgs AddressListArgs)
        {
            if (NewAddress != null)
                NewAddress(this, AddressListArgs);
        }

        protected int _NodeIndex;
        protected int _ParentIndex;
        protected clsGameContext _GameContext;
        protected NewChildrenEventArgs _NewChildrenArgs;
        public TrainerNode(
            int NodeIndex,
            clsGameContext GameContext,
            NewChildrenEventArgs Args)
        {
            _NodeIndex = NodeIndex;
            _ParentIndex = Args.ParentNodeIndex;
            _GameContext = GameContext;
            _NewChildrenArgs = Args;
        }
    }

    class nodeAddressList
        : IAddressNode
    {
        protected NewAddressListEventArgs _AddresInfo;

        public nodeAddressList(NewAddressListEventArgs Args)
        {
            _AddresInfo = Args;
        }

        public int ParentIndex { get { return _AddresInfo.ParentNodeIndex; } }
        public string Caption { get { return _AddresInfo.Caption; } }
        public UInt32 Address { get { return _AddresInfo.Address; } }
        public AddressListValueType ValueType { get { return _AddresInfo.ValueType; } }
        public int ValueScale { get { return _AddresInfo.ValueScale; } }
    }

    #endregion

    #region Main collection
    
    //////////////////////////////////////////////////////////////////////////    
    // To build a node tree, and create objects using factory pattern
    class clsGameTrainer
    {
        private List<TrainerNode> _AllTrainers = new List<TrainerNode>();
        private List<nodeAddressList> _AllAdress = new List<nodeAddressList>();
        
        private clsGameContext _GameContext;

        #region Enumerator & Index

        // Enumerator - Function
        internal class FunctionCollection
            : IEnumerable<ITrainerNode>
        {
            private List<TrainerNode> _AllTrainers;
            public FunctionCollection(List<TrainerNode> AllTrainers)
            {
                _AllTrainers = AllTrainers;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                lock (_AllTrainers)
                {
                    foreach (TrainerNode i in _AllTrainers)
                        yield return i as ITrainerNode;
                }
            }

            IEnumerator<ITrainerNode> IEnumerable<ITrainerNode>.GetEnumerator()
            {
                lock (_AllTrainers)
                {
                    foreach (TrainerNode i in _AllTrainers)
                        yield return i as ITrainerNode;
                }
            }
        }

        public FunctionCollection GetFunctionList()
        {
            return new FunctionCollection(_AllTrainers);
        }

        // Enumerator - Addresses
        internal class AddressCollection
            : IEnumerable<IAddressNode>
        {
            private List<nodeAddressList> _AllAddress;
            public AddressCollection(List<nodeAddressList> AllAddress)
            {
                _AllAddress = AllAddress;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                lock (_AllAddress)
                {
                    foreach (IAddressNode i in _AllAddress)
                        yield return i;
                }
            }

            IEnumerator<IAddressNode> IEnumerable<IAddressNode>.GetEnumerator()
            {
                lock (_AllAddress)
                {
                    foreach (IAddressNode i in _AllAddress)
                        yield return i as IAddressNode;
                }
            }
        }

        public AddressCollection GetAddressList()
        {
            return new AddressCollection(_AllAdress);
        }

        // Index - Function
        public ITrainerNode GetFunction(int Index)
        {
            return _AllTrainers[Index] as ITrainerNode;
        }

        // Index - Address
        public IAddressNode GetAddress(int Index)
        {
            return _AllAdress[Index];
        }

        #endregion

        // ctor()
        public clsGameTrainer(clsGameContext GameContext)
        {
            this._GameContext = GameContext;

            // Get trainers in 1st layer
            NewChildrenEventArgs Args = new NewChildrenEventArgs();
            Args.NodeType = TrainerNodeType.Root;
            NewChildrenEventHandler(this, Args);

            // Get all other trainers
            int Index = 0;
            int Count = 1;
            while (Index < Count)
            {
                _AllTrainers[Index].CreateChildren();
                lock (_AllTrainers)
                {
                    Index++;
                    Count = _AllTrainers.Count;
                }
            }
        }

        void NewChildrenEventHandler(object sender, NewChildrenEventArgs e)
        {
            lock (_AllTrainers)
            {
                TrainerNode newNode;
                int newNodeIndex = _AllTrainers.Count;

                // Factory pattern                
                switch (e.NodeType)
                {
                    case TrainerNodeType.Root:
                        newNode = new nodeRoot(newNodeIndex, _GameContext, e);
                        break;
                    case TrainerNodeType.Introduction:
                        newNode = new nodeIntroduction(newNodeIndex, _GameContext, e);
                        break;
                    case TrainerNodeType.Cash:
                        newNode = new nodeCash(newNodeIndex, _GameContext, e);
                        break;
                    case TrainerNodeType.AllSelectedUnit:
                        newNode = new nodeAllSelectedUnit(newNodeIndex, _GameContext, e);
                        break;
                    case TrainerNodeType.OneSelectedUnit:
                        newNode = new nodeOneSelectedUnit(newNodeIndex, _GameContext, e);
                        break;
                    case TrainerNodeType.UnitPropety:
                        newNode = new nodeUnitPropety(newNodeIndex, _GameContext, e);
                        break;
                    case TrainerNodeType.HeroPropety:
                        newNode = new nodeHeroPropety(newNodeIndex, _GameContext, e);
                        break;
                    //case TrainerNodeType.UnitAbility:
                    //    newNode = new nodeUnitAbility(newNodeIndex, _GameContext, e);
                    //    break;
                    case TrainerNodeType.AllUnitGoods:
                        newNode = new nodeAllUnitGoods(newNodeIndex, _GameContext, e);
                        break;
                    case TrainerNodeType.OneGoods:
                        newNode = new nodeOneGoods(newNodeIndex, _GameContext, e);
                        break;
                    default:
                        throw new System.ArgumentException();
                }
                newNode.NewChildren += NewChildrenEventHandler;
                newNode.NewAddress += NewAddressEventHandler;
                _AllTrainers.Add(newNode);
            }
        }

        private void NewAddressEventHandler(object sender, NewAddressListEventArgs e)
        {
            _AllAdress.Add(
                new nodeAddressList(e)
            );
        }
    }

    #endregion

    #region Every functions
    
    //////////////////////////////////////////////////////////////////////////
    // Concrete trainer
    class nodeRoot
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.Root; } }
        public string NodeTypeName { get { return "所有功能"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }

        public nodeRoot(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public override void CreateChildren()
        {
            // This will fill the following value(s):
            //      _NewChildrenArgs.pThisGame
            //      _NewChildrenArgs.pThisGameMemory
            War3Common.GetGameMemory(
                _GameContext,
                ref _NewChildrenArgs);
            
            base.CreateChild(TrainerNodeType.Introduction, NodeIndex);
            base.CreateChild(TrainerNodeType.Cash, NodeIndex);
            base.CreateChild(TrainerNodeType.AllSelectedUnit, NodeIndex);
        }
    }

    class nodeIntroduction
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.Introduction; } }
        public string NodeTypeName { get { return "使用方法"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }

        public nodeIntroduction(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public override void CreateChildren()
        {
        }
    }

    class nodeCash
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.Cash; } }
        public string NodeTypeName { get { return "游戏资源"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }

        public nodeCash(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public override void CreateChildren()
        {
            UInt32 UpperAddress;
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(_GameContext.ProcessID))
            {
                UpperAddress = War3Common.ReadFromGameMemory(
                    Mem, _GameContext, _NewChildrenArgs,
                    1) & 0xFFFF0000;
            }
            if (UpperAddress == 0)
                return;

            UInt32[] PlayerSourceBaseAddress = new UInt32[]
            {
                0,  // To skip index 0
                0x0190, 0x1410, 0x26A0, 0x3920, 0x4BB0,
                0x5E30, 0x70C0, 0x8350, 0x95D0, 0xA860,
                0xBAE0, 0xCD70
            };

            for (int i = 1; i <= 12; i++)
            {
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "P" + i.ToString() + " - 金",
                    unchecked(UpperAddress + PlayerSourceBaseAddress[i] + 0),
                    AddressListValueType.Integer,
                    10));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "P" + i.ToString() + " - 木",
                    unchecked(UpperAddress + PlayerSourceBaseAddress[i] + 0x80),
                    AddressListValueType.Integer,
                    10));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "P" + i.ToString() + " - 最大人口",
                    unchecked(UpperAddress + PlayerSourceBaseAddress[i] + 0x180),
                    AddressListValueType.Integer));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "P" + i.ToString() + " - 当前人口",
                    unchecked(UpperAddress + PlayerSourceBaseAddress[i] + 0x200),
                    AddressListValueType.Integer));
            }
        }
    }

    class nodeAllSelectedUnit
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.AllSelectedUnit; } }
        public string NodeTypeName { get { return "选中单位列表"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }
        
        public nodeAllSelectedUnit(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public override void CreateChildren()
        {
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(_GameContext.ProcessID))
            {
                // Get ESI of each selected unit
                UInt32 SelectedUnitList = Mem.ReadUInt32((IntPtr)_GameContext.War3AddressSelectedUnitList);
                UInt16 A2 = Mem.ReadUInt16((IntPtr)unchecked(SelectedUnitList + 0x28));
                UInt32 tmpAddress;
                tmpAddress = Mem.ReadUInt32((IntPtr)unchecked(SelectedUnitList + 0x58 + 4 * A2));
                tmpAddress = Mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x34));

                UInt32 ListHead   = Mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x1F0));
                // UInt32 ListEnd = Mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x1F4));
                UInt32 ListLength = Mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x1F8));

                UInt32 NextNode = ListHead;
                //UInt32 NextNodeNot = ~ListHead;
                for (int nSelectedUnitIndex = 0; nSelectedUnitIndex < ListLength; nSelectedUnitIndex++)
                {
                    _NewChildrenArgs.pThisUnit = Mem.ReadUInt32((IntPtr)unchecked(NextNode + 8));
                    // NextNodeNot             = Mem.ReadUInt32((IntPtr)unchecked(NextNode + 4));
                    NextNode                   = Mem.ReadUInt32((IntPtr)unchecked(NextNode + 0));

                    base.CreateChild(TrainerNodeType.OneSelectedUnit, NodeIndex);
                }
            }
        }
    }

    class nodeOneSelectedUnit
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.OneSelectedUnit; } }
        public string NodeTypeName
        {
            get
            {
                // return "单位";
                return "0x"
                    + _NewChildrenArgs.pThisUnit.ToString("X")
                    + ": "
                    + GetUnitName();
            }
        }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }
        
        public nodeOneSelectedUnit(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public string GetUnitName()
        {
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(_GameContext.ProcessID))
            {
                return Mem.ReadChar4((IntPtr)unchecked(_NewChildrenArgs.pThisUnit + 0x30));
            }
        }
                
        public override void CreateChildren()
        {
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(_GameContext.ProcessID))
            {
                _NewChildrenArgs.pUnitAttributes = Mem.ReadUInt32((IntPtr)unchecked(_NewChildrenArgs.pThisUnit + 0x1E4));
                _NewChildrenArgs.pHeroAttributes = Mem.ReadUInt32((IntPtr)unchecked(_NewChildrenArgs.pThisUnit + 0x1EC));

                if (_NewChildrenArgs.pUnitAttributes > 0)
                {
                    base.CreateChild(TrainerNodeType.UnitPropety, NodeIndex);
                    //base.CreateChild(TrainerNodeType.UnitAbility, NodeIndex);
                    base.CreateChild(TrainerNodeType.AllUnitGoods, NodeIndex);
                }

                if (_NewChildrenArgs.pHeroAttributes > 0)
                {
                    base.CreateChild(TrainerNodeType.HeroPropety, NodeIndex);
                }

                // Unit self propety(s)
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "单位名称",
                    unchecked(_NewChildrenArgs.pThisUnit + 0x30),
                    AddressListValueType.Char4));

                UInt32 tmpAddress1, tmpAddress2;
                Int32 tmpValue1, tmpValue2;
                tmpValue1 = Mem.ReadInt32((IntPtr)unchecked(_NewChildrenArgs.pThisUnit + 0x98 + 0x8));
                tmpAddress1 = War3Common.ReadFromGameMemory(
                    Mem, _GameContext, _NewChildrenArgs,
                    tmpValue1);
                tmpAddress1 = unchecked(tmpAddress1 + 0x84);
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "HP - 目前",
                    unchecked(tmpAddress1 - 0xC),
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "HP - 最大",
                    tmpAddress1,
                    AddressListValueType.Float));

                tmpValue1 = Mem.ReadInt32((IntPtr)unchecked(_NewChildrenArgs.pThisUnit + 0x98 + 0x28));
                tmpAddress1 = War3Common.ReadFromGameMemory(
                    Mem, _GameContext, _NewChildrenArgs,
                    tmpValue1);
                tmpAddress1 = unchecked(tmpAddress1 + 0x84);
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "MP - 目前",
                    unchecked(tmpAddress1 - 0xC),
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "MP - 最大",
                    tmpAddress1,
                    AddressListValueType.Float));

                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "盔甲 - 数量",
                    unchecked(_NewChildrenArgs.pThisUnit + 0xE0),
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "盔甲 - 种类",
                    unchecked(_NewChildrenArgs.pThisUnit + 0xE4),
                    AddressListValueType.Integer));
                
                // Move speed
                tmpAddress1 = unchecked(_NewChildrenArgs.pThisUnit + 0x1D8 - 0x24);
                do
                {
                    tmpValue1 = Mem.ReadInt32((IntPtr)unchecked(tmpAddress1 + 0x24));
                    tmpAddress1 = War3Common.ReadGameValue2(
                        Mem, _GameContext, _NewChildrenArgs,
                        tmpValue1);
                    tmpAddress2 = Mem.ReadUInt32((IntPtr)unchecked(tmpAddress1 + 0));
                    tmpValue1 = Mem.ReadInt32((IntPtr)unchecked(tmpAddress1 + 0x24));
                    tmpValue2 = Mem.ReadInt32((IntPtr)unchecked(tmpAddress1 + 0x28));

                    // Note: If new game version released, set breakpoint here
                    //       and check tmpAddress2. Set this value as War3AddressMoveSpeed
                    tmpAddress2 = Mem.ReadUInt32((IntPtr)unchecked(tmpAddress2 + 0x2D4));
                    if (_GameContext.War3AddressMoveSpeed == tmpAddress2)
                    {
                        CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                            "移动速度",
                            unchecked(tmpAddress1 + 0x78),
                            AddressListValueType.Float));
                    }
                } while (tmpValue1 > 0 && tmpValue2 > 0);

                // Coordinate
                tmpValue1 = Mem.ReadInt32((IntPtr)unchecked(_NewChildrenArgs.pThisUnit + 0x164 + 8));
                tmpAddress1 = War3Common.ReadGameValue1(
                    Mem, _GameContext, _NewChildrenArgs,
                    tmpValue1);
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "坐标 - X",
                    tmpAddress1,
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "坐标 - Y",
                    unchecked(tmpAddress1 + 4),
                    AddressListValueType.Float));
            }
        }
    }

    class nodeUnitPropety
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.UnitPropety; } }
        public string NodeTypeName { get { return "单位属性"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }

        public nodeUnitPropety(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public override void CreateChildren()
        {
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "攻击 - 频率",
                unchecked(_NewChildrenArgs.pUnitAttributes + 0x1B0),
                AddressListValueType.Float));
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "攻击 - 基础",
                unchecked(_NewChildrenArgs.pUnitAttributes + 0xA0),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "攻击 - 骰子",
                unchecked(_NewChildrenArgs.pUnitAttributes + 0x94),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "攻击 - 倍乘",
                unchecked(_NewChildrenArgs.pUnitAttributes + 0x88),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "攻击 - 种类",
                unchecked(_NewChildrenArgs.pUnitAttributes + 0xF4),
                AddressListValueType.Integer));
        }
    }

    class nodeHeroPropety
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.HeroPropety; } }
        public string NodeTypeName { get { return "英雄属性"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }
        
        public nodeHeroPropety(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }
        
        public override void CreateChildren()
        {
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "经验值",
                unchecked(_NewChildrenArgs.pHeroAttributes + 0x8C),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "力量",
                unchecked(_NewChildrenArgs.pHeroAttributes + 0x94),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "敏捷",
                unchecked(_NewChildrenArgs.pHeroAttributes + 0xA8),
                AddressListValueType.Integer));

            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(_GameContext.ProcessID))
            {
                UInt32 tmpAddress1;
                Int32 tmpValue1;
                tmpValue1 = Mem.ReadInt32((IntPtr)unchecked(_NewChildrenArgs.pHeroAttributes + 0x7C + 2 * 4));
                tmpAddress1 = War3Common.ReadGameValue1(
                    Mem, _GameContext, _NewChildrenArgs,
                    tmpValue1);
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "智力",
                    tmpAddress1,
                    AddressListValueType.Integer));
            }
            
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "可用技能点",
                unchecked(_NewChildrenArgs.pHeroAttributes + 0x90),
                AddressListValueType.Integer));

            for (UInt32 LearningAbilityIndex = 1; LearningAbilityIndex <= 5; LearningAbilityIndex++)
            {
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "学习技能" + LearningAbilityIndex.ToString() + " - 名称",
                    unchecked(_NewChildrenArgs.pHeroAttributes + 0xF0 + LearningAbilityIndex * 4),
                    AddressListValueType.Char4));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "学习技能" + LearningAbilityIndex.ToString() + " - 等级",
                    unchecked(_NewChildrenArgs.pHeroAttributes + 0x108 + LearningAbilityIndex * 4),
                    AddressListValueType.Integer));
                CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                    "学习技能" + LearningAbilityIndex.ToString() + " - 要求",
                    unchecked(_NewChildrenArgs.pHeroAttributes + 0x120 + LearningAbilityIndex * 4),
                    AddressListValueType.Integer));
            }
        }
    }
    /*
    class nodeUnitAbility
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.UnitAbility; } }
        public string NodeTypeName { get { return "单位技能"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }
        
        public nodeUnitAbility(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }
        
        public override void CreateChildren()
        {
        }
    }
    */
    class nodeAllUnitGoods
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.AllUnitGoods; } }
        public string NodeTypeName { get { return "物品列表"; } }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }

        public nodeAllUnitGoods(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public override void CreateChildren()
        {
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(_GameContext.ProcessID))
            {
                Int32 List = Mem.ReadInt32((IntPtr)unchecked(_NewChildrenArgs.pThisUnit + 0x1F4));
                if (List != 0)
                {
                    for (Int32 ItemIndex = 0; ItemIndex < 6; ItemIndex++)
                    {
                        UInt32 CurrentItem = 0;

                        // We assume ItemIndex never go out of bounds to the List
                        Int32 tmpValue1 = Mem.ReadInt32((IntPtr)unchecked(List + 0xC * ItemIndex + 0x70));
                        if (tmpValue1 > 0)
                        {
                            UInt32 RawItem = War3Common.ReadFromGameMemory(
                            Mem, _GameContext, _NewChildrenArgs,
                            tmpValue1);
                            if (RawItem != 0)
                            {
                                UInt32 tmpValue2 = Mem.ReadUInt32((IntPtr)unchecked(RawItem + 0x20));
                                if (tmpValue2 == 0)
                                    CurrentItem = Mem.ReadUInt32((IntPtr)unchecked(RawItem + 0x54));
                            }
                            if (CurrentItem != 0)
                            {
                                _NewChildrenArgs.pCurrentGoods = CurrentItem;
                                base.CreateChild(TrainerNodeType.OneGoods, NodeIndex);
                            }
                        }
                    }   // foreach items
                }   // Item list exists
            }   // Mem
        }   // CreateChildren()
    }   // class nodeAllUnitGoods

    class nodeOneGoods
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.OneGoods; } }
        public string NodeTypeName
        {
            get
            {
                // return "物品";
                return "0x"
                    + _NewChildrenArgs.pCurrentGoods.ToString("X")
                    + ": "
                    + GetItemName();
            }
        }

        public int NodeIndex { get { return _NodeIndex; } }
        public int ParentIndex { get { return _ParentIndex; } }

        private string GetItemName()
        {
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(_GameContext.ProcessID))
            {
                return Mem.ReadChar4((IntPtr)unchecked(_NewChildrenArgs.pCurrentGoods + 0x30));
            }
        }

        public nodeOneGoods(int NodeIndex, clsGameContext GameContext, NewChildrenEventArgs Args)
            : base(NodeIndex, GameContext, Args)
        {
        }

        public override void CreateChildren()
        {
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "物品名称",
                unchecked(_NewChildrenArgs.pCurrentGoods + 0x30),
                AddressListValueType.Char4));
            CreateAddress(new NewAddressListEventArgs(_NodeIndex,
                "使用次数",
                unchecked(_NewChildrenArgs.pCurrentGoods + 0x84),
                AddressListValueType.Integer));
         }
    }

    #endregion
}
