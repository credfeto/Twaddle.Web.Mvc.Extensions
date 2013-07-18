rem nuget spec

nuget pack -build Twaddle.Web.Mvc.Extensions.csproj -Prop Configuration=Release -IncludeReferencedProjects

rem nuget push Twaddle.Web.Mvc.Extensions.{version}.nupkg