using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.LogEvents.Intercepts;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    public interface ILogEventReporter
    {
        /// <summary>
        /// Gets the name of the log event reporter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the list of allowed event names. If the list of excluded event names is defined, this list will be ignored.
        /// </summary>
        List<string> AllowedEventNames { get; }

        /// <summary>
        /// Gets the list of excluded event names. If this list is defined, the list of allowed event names will be ignored.
        /// </summary>
        List<string> ExcludedEventNames { get; }

        bool IsAllowEventReport(string eventName);
        /// <summary>
        /// Changes the list of allowed event names.
        /// </summary>
        /// <param name="eventNames">The new list of allowed event names.</param>
        void ChangeAllowedEventNames(List<string> eventNames);

        /// <summary>
        /// Changes the list of excluded event names.
        /// </summary>
        /// <param name="eventNames">The new list of excluded event names.</param>
        void ChangeExcludedEventNames(List<string> eventNames);

        /// <summary>
        /// Gets the map of event names.
        /// </summary>
        Dictionary<string, string> EventNameMap { get; }

        /// <summary>
        /// Gets the map of event parameter names.
        /// </summary>
        Dictionary<string, string> EventParameterNameMap { get; }

        /// <summary>
        /// Changes the map of event names.
        /// </summary>
        /// <param name="eventNameMap">The new map of event names.</param>
        void ChangeEventNameMap(Dictionary<string, string> eventNameMap);

        /// <summary>
        /// Changes the map of event parameter names.
        /// </summary>
        /// <param name="eventParameterNameMap">The new map of event parameter names.</param>
        void ChangeEventParameterNameMap(Dictionary<string, string> eventParameterNameMap);

        /// <summary>
        /// Gets the list of required event parameter names.
        /// </summary>
        Dictionary<string, string> RequiredEventParameters { get; }

        /// <summary>
        /// Gets the common event parameters.
        /// </summary>
        Dictionary<string, string> CommonEventParameters { get; }

        /// <summary>
        /// Adds a common event parameter.
        /// </summary>
        /// <param name="eventParameterName">The name of the event parameter.</param>
        /// <param name="value">The value of the event parameter.</param>
        void AddCommonEventParameter(string eventParameterName, string value);

        /// <summary>
        /// Checks if the event interceptor is enabled in this log event reporter.
        /// </summary>
        /// <param name="name">The name of the event interceptor.</param>
        /// <returns>True if the event interceptor is enabled, false otherwise.</returns>
        bool IsEnableEventIntercept(string interceptTypeName);

        /// <summary>
        /// Logs an event. The event parameters are specific to this event, common event parameters do not need to be explicitly set.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="eventParameters">The parameters of the event.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask LogEvent(string name, Dictionary<string, string> eventParameters);
        void EnableEventIntercept<TEventIntercept>() where TEventIntercept : ILogEventIntercept;
        void AddRequiredParameters(Dictionary<string, string> requiredParameters, int timeout);
    }


    /*
    /// <summary>
    /// 事件上报器接口
    /// 每个事件上报器独立设置事件公共参数，这样的设计是考虑到有些服务提供商不需要某些公共参数，比如AppsFlyer就不需要设置设备信息，因为它上报事件的时候自带了设备信息
    /// 而有些自己搭建的事件上报服务器有需要设备信息
    /// 而对于有些公共参数，不能立即获取到的，比如AB分组需要等服务器返回，归因数据需要等到第三方归因平台返回，而这些公共参数是某些第三方服务（包括自建服务）必须要的
    /// 缺失的话会导致统计和分析数据的不准确，因此可以为每个事件上报器设置一个必须的参数，等这些公共参数都收集齐了，在上报到服务器，之前上报的事件先放到
    /// 本地的事件等待列表中
    /// 而有些配置和公共参数需要从服务器上获取新版本
    /// 在本地缓存带版本的配置
    /// </summary>
    public interface ILogEventReporter
    {
        /// <summary>
        /// Gets the name of the log event reporter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Changes the enabled event names.
        /// </summary>
        /// <param name="eventNames">The array of event names.</param>
        void ChangeEnabledEventNames(params string[] eventNames);

        /// <summary>
        /// Changes the event name map.
        /// </summary>
        /// <param name="eventNameMap">The new event name map.</param>
        void ChangeEventNameMap(Dictionary<string, string> eventNameMap);

        /// <summary>
        /// Changes the event parameter name map.
        /// </summary>
        /// <param name="eventParameterNameMap">The new event parameter name map.</param>
        void ChangeEventParameterNameMap(Dictionary<string, string> eventParameterNameMap);

        /// <summary>
        /// Adds a common event parameter.
        /// </summary>
        /// <param name="eventParameterName">The name of the event parameter.</param>
        /// <param name="value">The value of the event parameter.</param>
        void AddCommonEventParameter(string eventParameterName, string value);

        /// <summary>
        /// Adds an event intercept.
        /// </summary>
        /// <param name="logEventIntercept">The log event intercept to add.</param>
        void AddEventIntercept(ILogEventIntercept logEventIntercept);

        /// <summary>
        /// Adds an event parameter intercept.
        /// </summary>
        /// <param name="eventParameterIntercept">The event parameter intercept to add.</param>
        void AddEventParameterIntercept(ILogEventParameterIntercept eventParameterIntercept);

        bool IsEnableEventIntercept(string name);
        ILogEventInterceptConfiguration GetEventInterceptConfiguration(string name);

        bool IsEnableEventParameterIntercept(string name);
        ILogEventParameterInterceptConfiguration GetEventParameterInterceptConfiguration(string name);
        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="eventParameters">The parameters of the event.</param>
        /// <returns>A UniTask that represents the asynchronous logging operation.</returns>
        UniTask LogEvent(string name, Dictionary<string, string> eventParameters);

    }
    */
}
