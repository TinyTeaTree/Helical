using System;
using Core;
using Cysharp.Threading.Tasks;

namespace Factories
{
    public class CompositeChildFactory<ChildType> : BaseFactory
        where ChildType : BaseVisual
    {
        private readonly BaseFactory _originFactory;
        private readonly Type _type;
        private ChildType _child;


        public CompositeChildFactory(BaseFactory originFactory)
        {
            _originFactory = originFactory;
        }
        public override async UniTask<TypeVisual> Create<TypeVisual>()
        {
            if (typeof(TypeVisual) == typeof(ChildType))
            {
                if (_child == null)
                {
                    Notebook.NoteError($"Child {typeof(ChildType)} was not provided to the Composite Factory");
                }
                return _child as TypeVisual;
            }
            else
            {
                var originVisual = await _originFactory.Create<TypeVisual>();

                if (_child == null && originVisual != null)
                {
                    _child = originVisual.GetComponentInChildren<ChildType>();
                }

                return originVisual;
            }
            
        }
    }
}