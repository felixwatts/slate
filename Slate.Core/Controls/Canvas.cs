using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Slate.Core.Controls
{
    public class Canvas : SlateBase
    {
        private List<Panel> _panels;


        public override Point Size => _panels.Max(c => c.ParentRegion.BottomRight);

        public Canvas()
        {
            _panels = new List<Panel>();
        }

        public void AddControl(ISlate control, Point at)
        {            
            var panel = new Panel(control, at, _updates);
            _panels.Add(panel);

            _updates.OnNext(Update.SizeChanged);            
        }

        public void RemoveControl(ISlate control)
        {
            var panel = _panels.FirstOrDefault(p => object.ReferenceEquals(p.Control, control));
            if(panel == null) return;
            _panels.Remove(panel);
            panel.Dispose();

            _updates.OnNext(Update.SizeChanged);
        }

        public override Cell GetCell(Point at)
        {
            var panel = _panels.FirstOrDefault(p => p.ParentRegion.Contains(at));
            
            return panel?.GetCell(at) ?? null;
        }

        private class Panel : SlateMutation
        {
            private Point _offset;
            private IDisposable _disposable;

            public Region ParentRegion => new Region(_offset, _offset + _source.Size);

            public Panel(ISlate source, Point offset, ISubject<Update> sink) : base(source)
            {
                _offset = offset;
                _disposable = _updates.Subscribe(sink.OnNext);
            }

            public ISlate Control => _source;

            public override Cell GetCell(Point at)
            {
                return _source.GetCell(at - _offset);
            }

            public override void MouseDown(Point cell, MouseButton button, ModifierKeys modifierKeys)
            {
                _source.MouseDown(cell - _offset, button, modifierKeys);
            }

            public override void MouseUp(Point cell, MouseButton button, ModifierKeys modifierKeys)
            {
                _source.MouseUp(cell - _offset, button, modifierKeys);
            }

            public override void MouseMove(Point cell, ModifierKeys modifierKeys)
            {
                _source.MouseMove(cell - _offset, modifierKeys);
            }

            public override void SetVisibleRegions(Region[] visibleRegions)
            {
                _source.SetVisibleRegions(visibleRegions.Select(r => r.Translate(-_offset)).ToArray());
            }

            public override void Dispose()
            {
                _disposable.Dispose();
                base.Dispose();
            }

            protected override void HandleUpdate(Update update)
            {
                switch(update.Type)
                {
                    case UpdateType.RegionDirty:
                        _updates.OnNext(Update.RegionDirty(update.Region.Translate(_offset)));
                        break;
                    default: base.HandleUpdate(update); break;
                }
            }
        }
    }
}