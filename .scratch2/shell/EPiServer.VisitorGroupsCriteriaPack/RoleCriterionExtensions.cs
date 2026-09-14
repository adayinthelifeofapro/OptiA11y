using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.VisitorGroupsCriteriaPack;

internal static class RoleCriterionExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __TargetRoleCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1075944968, "TargetRole"), "Target role: {DecodedRoleName}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __TestingUserCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1699575032, "TestingUser"), "Testing user: {PrincipalIdentityName}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Target role: {DecodedRoleName}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void TargetRole(this ILogger<RoleCriterion> logger, string decodedRoleName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__TargetRoleCallback(logger, decodedRoleName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Testing user: {PrincipalIdentityName}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void TestingUser(this ILogger<RoleCriterion> logger, string principalIdentityName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__TestingUserCallback(logger, principalIdentityName, null);
		}
	}
}
