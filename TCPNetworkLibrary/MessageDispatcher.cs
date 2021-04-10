#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TCPNetworkModule;

namespace TCPNetworkModule
{
    public abstract class MessageDispatcher<TSerializedDataType> where TSerializedDataType : class, new()
    {
        //TODO: Change the Dispatcher system to a better mechanism.
        readonly List<(RouteAttribute route, Func<TSerializedDataType,Task<TSerializedDataType?>> targetMethod)> _handlers = new List<(RouteAttribute route, Func<TSerializedDataType, Task<TSerializedDataType?>> targetMethod)>();


        public void Bind<TProtocol>( Channel<TProtocol, TSerializedDataType> channel )
            where TProtocol : Protocol<TSerializedDataType>, new()
            => channel.OnMessage( async m => {
                var response = await DispatchAsync(m).ConfigureAwait(false);
                if ( response != null ) {
                    try 
                    {
                        await channel.SendAsync( response ).ConfigureAwait( false );
                    } 
                    catch ( Exception _e ) 
                    {
                        Console.WriteLine( $"Error Occur {_e}" );
                    }
                }
            } );

        public async Task<TSerializedDataType?> DispatchAsync( TSerializedDataType message )
        {
            foreach ( var (route, target) in _handlers ) 
            {
                if ( IsMatch( route, message ) ) 
                {
                    return await target( message );
                }
            }

            //No handler?? what to do??
            return null;
        }

        protected void AddHandler( RouteAttribute route, Func<TSerializedDataType, Task<TSerializedDataType?>> targetMethod )
            => _handlers.Add( (route, targetMethod) );

        protected abstract bool IsMatch( RouteAttribute route, TSerializedDataType message );

        public virtual void Register<TParam, TResult>( Func<TParam, Task<TResult>> target )
        {
            if ( !HasAttribute( target.Method ) )
                throw new Exception( "Missing Required Route Attribute" );

            var wrapper = new Func<TSerializedDataType,Task<TSerializedDataType?>>( async xml => {
                var @param = Deserialize<TParam>(xml);
                var result = await target(@param);

                if(result != null)
                    return Serialize<TResult>(result);
                else
                    return null;
            });

#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler( GetRouteAttribute( target.Method ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.
        }


        protected abstract TParam Deserialize<TParam>( TSerializedDataType message );
        protected abstract object Deserialize( Type paramType, TSerializedDataType message );

        protected abstract TSerializedDataType? Serialize<T>( T instance );

        public virtual void Register<TParam>( Func<TParam, Task> target )
        {
            if ( !HasAttribute( target.Method ) )
                throw new Exception( "Missing Required Route Attribute" );

            var wrapper = new Func<TSerializedDataType,Task<TSerializedDataType?>>( async xml => {
                var @param = Deserialize<TParam>(xml);
                await target(@param);
                return null;
            });

#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler( GetRouteAttribute( target.Method ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.
        }

        protected bool HasAttribute( MethodInfo mi ) => GetRouteAttribute( mi ) != null;
        protected abstract RouteAttribute? GetRouteAttribute( MethodInfo mi );


    }
}
