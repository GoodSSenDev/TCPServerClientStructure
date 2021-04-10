﻿#nullable enable

using System;
using System.Reflection;

using Newtonsoft.Json.Linq;
using TCPNetworkModule;

namespace TCPNetworkModule.Json
{
    public class JsonMessageDispatcher : MessageDispatcher<JObject>
    {

        protected override TMessage Deserialize<TMessage>( JObject message )
             => JsonSerialization.Deserialize<TMessage>( message );

        protected override object Deserialize( Type paramType, JObject message )
            => JsonSerialization.ToObject( paramType, message );

        protected override RouteAttribute? GetRouteAttribute( MethodInfo mi )
            => mi.GetCustomAttribute<JsonRouteAttribute>( );

        protected override bool IsMatch( RouteAttribute route, JObject message )
            => message.SelectToken( route.Path )?.ToString( ) == ( route as JsonRouteAttribute )?.Value;

        protected override JObject? Serialize<TMessage>( TMessage instance )
            => JsonSerialization.Serialize( instance );
    }
}
