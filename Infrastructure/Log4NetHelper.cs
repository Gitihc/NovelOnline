using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Infrastructure
{

    public class LoggerProperties
    {
        public static ILoggerRepository LogRepository { get; set; }
    }
    public static class Logger
    {
        private static ILog _logger;

        static Logger()
        {
            if (_logger == null)
            {
                //Console.OutputEncoding = System.Text.Encoding.UTF8;
                //var repository = LogManager.CreateRepository("NETCoreRepository");
                ////log4net从log4net.config文件中读取配置信息
                //XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                _logger = LogManager.GetLogger(LoggerProperties.LogRepository.Name, Assembly.GetExecutingAssembly().FullName);
            }
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(string message, Exception exception = null)
        {
            if (exception == null)
                _logger.Info(message);
            else
                _logger.Info(message, exception);
        }

        public static void Info(Type t, string message, Exception exception = null)
        {
            ILog log = LogManager.GetLogger(LoggerProperties.LogRepository.Name, t);
            if (exception == null)
                log.Info(message);
            else
                log.Info(message, exception);

        }

        /// <summary>
        /// 告警日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(string message, Exception exception = null)
        {
            if (exception == null)
                _logger.Warn(message);
            else
                _logger.Warn(message, exception);
        }

        public static void Warn(Type t, string message, Exception exception = null)
        {
            ILog log = LogManager.GetLogger(LoggerProperties.LogRepository.Name, t);
            if (exception == null)
                log.Warn(message);
            else
                log.Warn(message, exception);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(string message, Exception exception = null)
        {
            if (exception == null)
                _logger.Error(message);
            else
                _logger.Error(message, exception);
        }
        public static void Error(Type t, string message, Exception exception = null)
        {
            ILog log = LogManager.GetLogger(LoggerProperties.LogRepository.Name, t);
            if (exception == null)
                log.Error(message);
            else
                log.Error(message, exception);
        }

    }
}
