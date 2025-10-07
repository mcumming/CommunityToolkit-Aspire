using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.NuGetServer;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for NuGetServer resources to an <see cref="IDistributedApplicationBuilder"/>.
/// </summary>
public static class NuGetServerHostingExtensions
{
    /// <summary>
    /// Configures the host port that the NuGetServer resource is exposed on instead of using randomly assigned port.
    /// </summary>
    /// <param name="builder">The resource builder for NuGetServer.</param>
    /// <param name="port">The port to bind on the host. If <see langword="null"/> is used random port will be assigned.</param>
    /// <returns>The resource builder for NuGetServer.</returns>
    public static IResourceBuilder<NuGetServerContainerResource> WithHostPort(this IResourceBuilder<NuGetServerContainerResource> builder, int? port)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithEndpoint(NuGetServerContainerResource.PrimaryEndpointName, endpoint =>
        {
            endpoint.Port = port;
        });
    }

    /// <summary>
    /// Adds a NuGetServer container resource to the application.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="port">The host port to bind the underlying container to.</param>
    /// <remarks>
    /// Multiple <see cref="AddNuGetServer(IDistributedApplicationBuilder, string, int?)"/> calls will return the same resource builder instance.
    /// </remarks>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<NuGetServerContainerResource> AddNuGetServer(this IDistributedApplicationBuilder builder, [ResourceName] string name, int? port = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);

        if (builder.Resources.OfType<NuGetServerContainerResource>().SingleOrDefault() is { } existingNuGetServerResource)
        {
            var builderForExistingResource = builder.CreateResourceBuilder(existingNuGetServerResource);
            return builderForExistingResource;
        }
        else
        {
            var nugetServerContainer = new NuGetServerContainerResource(name);
            var nugetServerContainerBuilder = builder.AddResource(nugetServerContainer)
                                               .WithImage(NuGetServerContainerImageTags.Image)
                                               .WithImageTag(NuGetServerContainerImageTags.Tag)
                                               .WithImageRegistry(NuGetServerContainerImageTags.Registry)
                                               .WithHttpEndpoint(targetPort: 8080, port: port, name: NuGetServerContainerResource.PrimaryEndpointName)
                                               .WithUrlForEndpoint(NuGetServerContainerResource.PrimaryEndpointName, annotation => annotation.Url += $"/v3/index.json");

            return nugetServerContainerBuilder;
        }
    }
}