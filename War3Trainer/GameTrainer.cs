using System;
using System.Collections.Generic;

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
        : EventArgs
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

        public NewChildrenEventArgs Clone()
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
            int parentNodeIndex,
            string caption,
            UInt32 address,
            AddressListValueType valueType)
            : this(parentNodeIndex, caption, address, valueType, 1)
        {
        }

        public NewAddressListEventArgs(
            int parentNodeIndex,
            string caption,
            UInt32 address,
            AddressListValueType valueType,
            int valueScale)
        {
            this.ParentNodeIndex = parentNodeIndex;
            this.Caption = caption;

            this.Address = address;
            this.ValueType = valueType;
            this.ValueScale = valueScale;
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
            TrainerNodeType childType,
            int parentIndex)
        {
            if (NewChildren != null)
            {
                NewChildrenEventArgs args = _newChildrenArgs.Clone();
                args.NodeType = childType;
                args.ParentNodeIndex = parentIndex;
                NewChildren(this, args);
            }
        }

        protected void CreateAddress(NewAddressListEventArgs AddressListArgs)
        {
            if (NewAddress != null)
                NewAddress(this, AddressListArgs);
        }

        protected int _nodeIndex;
        protected int _parentIndex;
        protected GameContext _gameContext;
        protected NewChildrenEventArgs _newChildrenArgs;

        public TrainerNode(
            int nodeIndex,
            GameContext gameContext,
            NewChildrenEventArgs args)
        {
            _nodeIndex = nodeIndex;
            _parentIndex = args.ParentNodeIndex;
            _gameContext = gameContext;
            _newChildrenArgs = args;
        }
    }

    class NodeAddressList
        : IAddressNode
    {
        protected NewAddressListEventArgs _AddresInfo;

        public NodeAddressList(NewAddressListEventArgs args)
        {
            _AddresInfo = args;
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
    class GameTrainer
    {
        private List<TrainerNode> _allTrainers = new List<TrainerNode>();
        private List<NodeAddressList> _allAdress = new List<NodeAddressList>();
        
        private GameContext _gameContext;

        #region Enumerator & Index

        // Enumerator - Function
        internal class FunctionCollection
            : IEnumerable<ITrainerNode>
        {
            private List<TrainerNode> _allTrainers;

            public FunctionCollection(List<TrainerNode> allTrainers)
            {
                _allTrainers = allTrainers;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                lock (_allTrainers)
                {
                    foreach (TrainerNode i in _allTrainers)
                        yield return i as ITrainerNode;
                }
            }

            IEnumerator<ITrainerNode> IEnumerable<ITrainerNode>.GetEnumerator()
            {
                lock (_allTrainers)
                {
                    foreach (TrainerNode i in _allTrainers)
                        yield return i as ITrainerNode;
                }
            }
        }

        public FunctionCollection GetFunctionList()
        {
            return new FunctionCollection(_allTrainers);
        }

        // Enumerator - Addresses
        internal class AddressCollection
            : IEnumerable<IAddressNode>
        {
            private List<NodeAddressList> _AllAddress;
            public AddressCollection(List<NodeAddressList> AllAddress)
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
            return new AddressCollection(_allAdress);
        }

        // Index - Function
        public ITrainerNode GetFunction(int index)
        {
            return _allTrainers[index] as ITrainerNode;
        }

        // Index - Address
        public IAddressNode GetAddress(int index)
        {
            return _allAdress[index];
        }

        #endregion

        // ctor()
        public GameTrainer(GameContext gameContext)
        {
            this._gameContext = gameContext;

            // Get trainers in 1st layer
            NewChildrenEventArgs args = new NewChildrenEventArgs();
            args.NodeType = TrainerNodeType.Root;
            NewChildrenEventHandler(this, args);

            // Get all other trainers
            int Index = 0;
            int Count = 1;
            while (Index < Count)
            {
                _allTrainers[Index].CreateChildren();
                lock (_allTrainers)
                {
                    Index++;
                    Count = _allTrainers.Count;
                }
            }
        }

        void NewChildrenEventHandler(object sender, NewChildrenEventArgs e)
        {
            lock (_allTrainers)
            {
                TrainerNode newNode;
                int newNodeIndex = _allTrainers.Count;

                // Factory pattern                
                switch (e.NodeType)
                {
                    case TrainerNodeType.Root:
                        newNode = new NodeRoot(newNodeIndex, _gameContext, e);
                        break;
                    case TrainerNodeType.Introduction:
                        newNode = new NodeIntroduction(newNodeIndex, _gameContext, e);
                        break;
                    case TrainerNodeType.Cash:
                        newNode = new NodeCash(newNodeIndex, _gameContext, e);
                        break;
                    case TrainerNodeType.AllSelectedUnit:
                        newNode = new NodeAllSelectedUnit(newNodeIndex, _gameContext, e);
                        break;
                    case TrainerNodeType.OneSelectedUnit:
                        newNode = new NodeOneSelectedUnit(newNodeIndex, _gameContext, e);
                        break;
                    case TrainerNodeType.UnitPropety:
                        newNode = new NodeUnitPropety(newNodeIndex, _gameContext, e);
                        break;
                    case TrainerNodeType.HeroPropety:
                        newNode = new NodeHeroPropety(newNodeIndex, _gameContext, e);
                        break;
                    //case TrainerNodeType.UnitAbility:
                    //    newNode = new nodeUnitAbility(newNodeIndex, _GameContext, e);
                    //    break;
                    case TrainerNodeType.AllUnitGoods:
                        newNode = new NodeAllUnitGoods(newNodeIndex, _gameContext, e);
                        break;
                    case TrainerNodeType.OneGoods:
                        newNode = new NodeOneGoods(newNodeIndex, _gameContext, e);
                        break;
                    default:
                        throw new System.ArgumentException();
                }
                newNode.NewChildren += NewChildrenEventHandler;
                newNode.NewAddress += NewAddressEventHandler;
                _allTrainers.Add(newNode);
            }
        }

        private void NewAddressEventHandler(object sender, NewAddressListEventArgs e)
        {
            _allAdress.Add(
                new NodeAddressList(e)
            );
        }
    }

    #endregion

    #region Every functions
    
    //////////////////////////////////////////////////////////////////////////
    // Concrete trainer
    class NodeRoot
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.Root; } }
        public string NodeTypeName { get { return "所有功能"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }

        public NodeRoot(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public override void CreateChildren()
        {
            // This will fill the following value(s):
            //      _NewChildrenArgs.pThisGame
            //      _NewChildrenArgs.pThisGameMemory
            War3Common.GetGameMemory(
                _gameContext,
                ref _newChildrenArgs);
            
            base.CreateChild(TrainerNodeType.Introduction, NodeIndex);
            base.CreateChild(TrainerNodeType.Cash, NodeIndex);
            base.CreateChild(TrainerNodeType.AllSelectedUnit, NodeIndex);
        }
    }

    class NodeIntroduction
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.Introduction; } }
        public string NodeTypeName { get { return "使用方法"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }

        public NodeIntroduction(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public override void CreateChildren()
        {
        }
    }

    class NodeCash
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.Cash; } }
        public string NodeTypeName { get { return "游戏资源"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }

        public NodeCash(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public override void CreateChildren()
        {
            UInt32 upperAddress;
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_gameContext.ProcessId))
            {
                upperAddress = War3Common.ReadFromGameMemory(
                    mem, _gameContext, _newChildrenArgs,
                    1) & 0xFFFF0000;
            }
            if (upperAddress == 0)
                return;

            UInt32[] playerSourceBaseAddress = new UInt32[]
            {
                0,  // To skip index 0
                0x0190, 0x1410, 0x26A0, 0x3920, 0x4BB0,
                0x5E30, 0x70C0, 0x8350, 0x95D0, 0xA860,
                0xBAE0, 0xCD70
            };

            for (int i = 1; i <= 12; i++)
            {
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "P" + i.ToString() + " - 金",
                    unchecked(upperAddress + playerSourceBaseAddress[i] + 0),
                    AddressListValueType.Integer,
                    10));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "P" + i.ToString() + " - 木",
                    unchecked(upperAddress + playerSourceBaseAddress[i] + 0x80),
                    AddressListValueType.Integer,
                    10));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "P" + i.ToString() + " - 最大人口",
                    unchecked(upperAddress + playerSourceBaseAddress[i] + 0x180),
                    AddressListValueType.Integer));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "P" + i.ToString() + " - 当前人口",
                    unchecked(upperAddress + playerSourceBaseAddress[i] + 0x200),
                    AddressListValueType.Integer));
            }
        }
    }

    class NodeAllSelectedUnit
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.AllSelectedUnit; } }
        public string NodeTypeName { get { return "选中单位列表"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }
        
        public NodeAllSelectedUnit(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public override void CreateChildren()
        {
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_gameContext.ProcessId))
            {
                // Get ESI of each selected unit
                UInt32 selectedUnitList = mem.ReadUInt32((IntPtr)_gameContext.War3AddressSelectedUnitList);
                UInt16 a2 = mem.ReadUInt16((IntPtr)unchecked(selectedUnitList + 0x28));
                UInt32 tmpAddress;
                tmpAddress = mem.ReadUInt32((IntPtr)unchecked(selectedUnitList + 0x58 + 4 * a2));
                tmpAddress = mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x34));

                UInt32 listHead   = mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x1F0));
                // UInt32 ListEnd = mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x1F4));
                UInt32 listLength = mem.ReadUInt32((IntPtr)unchecked(tmpAddress + 0x1F8));

                UInt32 nextNode = listHead;
                //UInt32 NextNodeNot = ~ListHead;
                for (int selectedUnitIndex = 0; selectedUnitIndex < listLength; selectedUnitIndex++)
                {
                    _newChildrenArgs.pThisUnit = mem.ReadUInt32((IntPtr)unchecked(nextNode + 8));
                    // NextNodeNot             = mem.ReadUInt32((IntPtr)unchecked(NextNode + 4));
                    nextNode                   = mem.ReadUInt32((IntPtr)unchecked(nextNode + 0));

                    base.CreateChild(TrainerNodeType.OneSelectedUnit, NodeIndex);
                }
            }
        }
    }

    class NodeOneSelectedUnit
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.OneSelectedUnit; } }
        public string NodeTypeName
        {
            get
            {
                // return "单位";
                return "0x"
                    + _newChildrenArgs.pThisUnit.ToString("X")
                    + ": "
                    + GetUnitName();
            }
        }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }
        
        public NodeOneSelectedUnit(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public string GetUnitName()
        {
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_gameContext.ProcessId))
            {
                return mem.ReadChar4((IntPtr)unchecked(_newChildrenArgs.pThisUnit + 0x30));
            }
        }
                
        public override void CreateChildren()
        {
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_gameContext.ProcessId))
            {
                _newChildrenArgs.pUnitAttributes = mem.ReadUInt32((IntPtr)unchecked(_newChildrenArgs.pThisUnit + _gameContext.War3OffsetUnitAttributes));
                _newChildrenArgs.pHeroAttributes = mem.ReadUInt32((IntPtr)unchecked(_newChildrenArgs.pThisUnit + _gameContext.War3OffsetHeroAttributes));

                if (_newChildrenArgs.pUnitAttributes > 0)
                {
                    base.CreateChild(TrainerNodeType.UnitPropety, NodeIndex);
                    //base.CreateChild(TrainerNodeType.UnitAbility, NodeIndex);
                    base.CreateChild(TrainerNodeType.AllUnitGoods, NodeIndex);
                }

                if (_newChildrenArgs.pHeroAttributes > 0)
                {
                    base.CreateChild(TrainerNodeType.HeroPropety, NodeIndex);
                }

                // Unit self propety(s)
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "单位名称",
                    unchecked(_newChildrenArgs.pThisUnit + 0x30),
                    AddressListValueType.Char4));

                UInt32 tmpAddress1, tmpAddress2;
                Int32 tmpValue1, tmpValue2;
                tmpValue1 = mem.ReadInt32((IntPtr)unchecked(_newChildrenArgs.pThisUnit + 0x98 + 0x8));
                tmpAddress1 = War3Common.ReadFromGameMemory(
                    mem, _gameContext, _newChildrenArgs,
                    tmpValue1);
                tmpAddress1 = unchecked(tmpAddress1 + 0x84);
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "HP - 目前",
                    unchecked(tmpAddress1 - 0xC),
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "HP - 最大",
                    tmpAddress1,
                    AddressListValueType.Float));

                tmpValue1 = mem.ReadInt32((IntPtr)unchecked(_newChildrenArgs.pThisUnit + 0x98 + 0x28));
                tmpAddress1 = War3Common.ReadFromGameMemory(
                    mem, _gameContext, _newChildrenArgs,
                    tmpValue1);
                tmpAddress1 = unchecked(tmpAddress1 + 0x84);
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "MP - 目前",
                    unchecked(tmpAddress1 - 0xC),
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "MP - 最大",
                    tmpAddress1,
                    AddressListValueType.Float));

                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "盔甲 - 数量",
                    unchecked(_newChildrenArgs.pThisUnit + 0xE0),
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "盔甲 - 种类",
                    unchecked(_newChildrenArgs.pThisUnit + 0xE4),
                    AddressListValueType.Integer));
                
                // Move speed
                tmpAddress1 = unchecked(_newChildrenArgs.pThisUnit + _gameContext.War3OffsetMoveSpeed - 0x24);
                do
                {
                    tmpValue1 = mem.ReadInt32((IntPtr)unchecked(tmpAddress1 + 0x24));
                    tmpAddress1 = War3Common.ReadGameValue2(
                        mem, _gameContext, _newChildrenArgs,
                        tmpValue1);
                    tmpAddress2 = mem.ReadUInt32((IntPtr)unchecked(tmpAddress1 + 0));
                    tmpValue1 = mem.ReadInt32((IntPtr)unchecked(tmpAddress1 + 0x24));
                    tmpValue2 = mem.ReadInt32((IntPtr)unchecked(tmpAddress1 + 0x28));

                    // Note: If new game version released, set breakpoint here
                    //       and check tmpAddress2. Set this value as War3AddressMoveSpeed
                    tmpAddress2 = mem.ReadUInt32((IntPtr)unchecked(tmpAddress2 + 0x2D4));
                    if (_gameContext.War3AddressMoveSpeed == tmpAddress2)
                    {
                        CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                            "移动速度",
                            unchecked(tmpAddress1 + 0x70),  // +70 or +78 are both OK
                            AddressListValueType.Float));
                    }
                } while (tmpValue1 > 0 && tmpValue2 > 0);

                // Coordinate
                tmpValue1 = mem.ReadInt32((IntPtr)unchecked(_newChildrenArgs.pThisUnit + 0x164 + 8));
                tmpAddress1 = War3Common.ReadGameValue1(
                    mem, _gameContext, _newChildrenArgs,
                    tmpValue1);
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "坐标 - X",
                    tmpAddress1,
                    AddressListValueType.Float));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "坐标 - Y",
                    unchecked(tmpAddress1 + 4),
                    AddressListValueType.Float));
            }
        }
    }

    class NodeUnitPropety
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.UnitPropety; } }
        public string NodeTypeName { get { return "单位属性"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }

        public NodeUnitPropety(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public override void CreateChildren()
        {
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "攻击 - 频率",
                unchecked(_newChildrenArgs.pUnitAttributes + 0x1B0),
                AddressListValueType.Float));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "攻击 - 基础",
                unchecked(_newChildrenArgs.pUnitAttributes + 0xA0),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "攻击 - 骰子",
                unchecked(_newChildrenArgs.pUnitAttributes + 0x94),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "攻击 - 倍乘",
                unchecked(_newChildrenArgs.pUnitAttributes + 0x88),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "攻击 - 种类",
                unchecked(_newChildrenArgs.pUnitAttributes + 0xF4),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "攻击 - 范围1",
                unchecked(_newChildrenArgs.pUnitAttributes + 0x258 + 0 * 8),
                AddressListValueType.Float));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "攻击 - 范围2",
                unchecked(_newChildrenArgs.pUnitAttributes + 0x258 + 1 * 8),
                AddressListValueType.Float));
        }
    }

    class NodeHeroPropety
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.HeroPropety; } }
        public string NodeTypeName { get { return "英雄属性"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }
        
        public NodeHeroPropety(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }
        
        public override void CreateChildren()
        {
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "经验值",
                unchecked(_newChildrenArgs.pHeroAttributes + 0x8C),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "力量",
                unchecked(_newChildrenArgs.pHeroAttributes + 0x94),
                AddressListValueType.Integer));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "敏捷",
                unchecked(_newChildrenArgs.pHeroAttributes + 0xA8),
                AddressListValueType.Integer));

            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_gameContext.ProcessId))
            {
                UInt32 tmpAddress1;
                Int32 tmpValue1;
                tmpValue1 = mem.ReadInt32((IntPtr)unchecked(_newChildrenArgs.pHeroAttributes + 0x7C + 2 * 4));
                tmpAddress1 = War3Common.ReadGameValue1(
                    mem, _gameContext, _newChildrenArgs,
                    tmpValue1);
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "智力",
                    tmpAddress1,
                    AddressListValueType.Integer));
            }
            
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "可用技能点",
                unchecked(_newChildrenArgs.pHeroAttributes + 0x90),
                AddressListValueType.Integer));

            for (UInt32 LearningAbilityIndex = 1; LearningAbilityIndex <= 5; LearningAbilityIndex++)
            {
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "学习技能" + LearningAbilityIndex.ToString() + " - 名称",
                    unchecked(_newChildrenArgs.pHeroAttributes + 0xF0 + LearningAbilityIndex * 4),
                    AddressListValueType.Char4));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "学习技能" + LearningAbilityIndex.ToString() + " - 等级",
                    unchecked(_newChildrenArgs.pHeroAttributes + 0x108 + LearningAbilityIndex * 4),
                    AddressListValueType.Integer));
                CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                    "学习技能" + LearningAbilityIndex.ToString() + " - 要求",
                    unchecked(_newChildrenArgs.pHeroAttributes + 0x120 + LearningAbilityIndex * 4),
                    AddressListValueType.Integer));
            }
        }
    }
    /*
    class NodeUnitAbility
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.UnitAbility; } }
        public string NodeTypeName { get { return "单位技能"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }
        
        public NodeUnitAbility(int nodeIndex, clsGameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }
        
        public override void CreateChildren()
        {
        }
    }
    */
    class NodeAllUnitGoods
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.AllUnitGoods; } }
        public string NodeTypeName { get { return "物品列表"; } }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }

        public NodeAllUnitGoods(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public override void CreateChildren()
        {
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_gameContext.ProcessId))
            {
                Int32 list = mem.ReadInt32((IntPtr)unchecked(_newChildrenArgs.pThisUnit + _gameContext.War3OffsetGoodsList));
                if (list != 0)
                {
                    for (Int32 itemIndex = 0; itemIndex < 6; itemIndex++)
                    {
                        UInt32 currentItem = 0;

                        // We assume ItemIndex never go out of bounds to the List
                        Int32 tmpValue1 = mem.ReadInt32((IntPtr)unchecked(list + 0xC * itemIndex + 0x70));
                        if (tmpValue1 > 0)
                        {
                            UInt32 RawItem = War3Common.ReadFromGameMemory(
                            mem, _gameContext, _newChildrenArgs,
                            tmpValue1);
                            if (RawItem != 0)
                            {
                                UInt32 tmpValue2 = mem.ReadUInt32((IntPtr)unchecked(RawItem + 0x20));
                                if (tmpValue2 == 0)
                                    currentItem = mem.ReadUInt32((IntPtr)unchecked(RawItem + 0x54));
                            }
                            if (currentItem != 0)
                            {
                                _newChildrenArgs.pCurrentGoods = currentItem;
                                base.CreateChild(TrainerNodeType.OneGoods, NodeIndex);
                            }
                        }
                    }   // foreach items
                }   // Item list exists
            }   // mem
        }   // CreateChildren()
    }

    class NodeOneGoods
        : TrainerNode, ITrainerNode
    {
        public TrainerNodeType NodeType { get { return TrainerNodeType.OneGoods; } }
        public string NodeTypeName
        {
            get
            {
                // return "物品";
                return "0x"
                    + _newChildrenArgs.pCurrentGoods.ToString("X")
                    + ": "
                    + GetItemName();
            }
        }

        public int NodeIndex { get { return _nodeIndex; } }
        public int ParentIndex { get { return _parentIndex; } }

        private string GetItemName()
        {
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_gameContext.ProcessId))
            {
                return mem.ReadChar4((IntPtr)unchecked(_newChildrenArgs.pCurrentGoods + 0x30));
            }
        }

        public NodeOneGoods(int nodeIndex, GameContext gameContext, NewChildrenEventArgs args)
            : base(nodeIndex, gameContext, args)
        {
        }

        public override void CreateChildren()
        {
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "物品名称",
                unchecked(_newChildrenArgs.pCurrentGoods + 0x30),
                AddressListValueType.Char4));
            CreateAddress(new NewAddressListEventArgs(_nodeIndex,
                "使用次数",
                unchecked(_newChildrenArgs.pCurrentGoods + 0x84),
                AddressListValueType.Integer));
         }
    }

    #endregion
}
