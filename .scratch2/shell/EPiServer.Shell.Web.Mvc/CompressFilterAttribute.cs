using System.IO.Compression;
using System.Threading.Tasks;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Web.Mvc;

public class CompressFilterAttribute : ResultFilterAttribute
{
	private class CompressionOptions : IOptions<ResponseCompressionOptions>
	{
		private class GZipCompressionProviderOptionsProvider : IOptions<GzipCompressionProviderOptions>
		{
			public GzipCompressionProviderOptions Value { get; }

			public GZipCompressionProviderOptionsProvider()
			{
				Value = new GzipCompressionProviderOptions
				{
					Level = CompressionLevel.Fastest
				};
			}
		}

		private class BrotliCompressionProviderOptionsProvider : IOptions<BrotliCompressionProviderOptions>
		{
			public BrotliCompressionProviderOptions Value { get; }

			public BrotliCompressionProviderOptionsProvider()
			{
				Value = new BrotliCompressionProviderOptions
				{
					Level = CompressionLevel.Fastest
				};
			}
		}

		public ResponseCompressionOptions Value { get; }

		public CompressionOptions(string acceptEncoding)
		{
			Value = new ResponseCompressionOptions
			{
				EnableForHttps = true
			};
			acceptEncoding = acceptEncoding.ToUpperInvariant();
			if (acceptEncoding.Contains("GZIP"))
			{
				Value.Providers.Add(new GzipCompressionProvider(new GZipCompressionProviderOptionsProvider()));
			}
			else
			{
				Value.Providers.Add(new BrotliCompressionProvider(new BrotliCompressionProviderOptionsProvider()));
			}
		}
	}

	private const CompressionLevel DefaultCompressionLevel = CompressionLevel.Fastest;

	private bool? _enabled;

	internal bool Enabled
	{
		get
		{
			bool? enabled = _enabled;
			if (!enabled.HasValue)
			{
				bool? flag = (_enabled = ServiceProviderExtensions.GetInstance<ClientResourceOptions>(ServiceLocator.Current).Compress);
				return flag == true;
			}
			return enabled == true;
		}
		set
		{
			_enabled = value;
		}
	}

	public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		HttpRequest request = context.HttpContext.Request;
		if (!Enabled || context.HttpContext.Response.StatusCode != 200)
		{
			await next();
			return;
		}
		StringValues acceptEncoding = request.Headers.AcceptEncoding;
		if (string.IsNullOrEmpty(acceptEncoding))
		{
			await next();
			return;
		}
		await new ResponseCompressionMiddleware(async delegate
		{
			await next();
		}, new ResponseCompressionProvider(context.HttpContext.RequestServices, new CompressionOptions(acceptEncoding))).Invoke(context.HttpContext);
	}
}
