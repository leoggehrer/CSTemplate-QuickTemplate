//@BaseCode
//MdStart
using QuickTemplate.Logic.DataContext;

namespace QuickTemplate.Logic.Controllers
{
    public abstract partial class ControllerObject : IDisposable
    {
        static ControllerObject()
        {
            BeforeClassInitialize();
            AfterClassInitialize();
        }
        static partial void BeforeClassInitialize();
        static partial void AfterClassInitialize();

        private readonly bool contextOwner;
        internal ProjectDbContext? Context { get; private set; }

        internal ControllerObject(ProjectDbContext context)
        {
            Context = context;
            contextOwner = true;
        }
        internal ControllerObject(ControllerObject other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            if (other.Context == null)
            {
                throw new Exception("The context from the other controller must not be null.");
            }
            Context = other.Context;
            contextOwner = false;
        }

        #region Dispose pattern
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    if (contextOwner)
                    {
                        Context?.Dispose();
                    }
                    Context = null;
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ControllerObject()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose pattern
    }
}
//MdEnd