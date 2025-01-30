FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV TZ='Europe/Moscow'
RUN apt-get update && \
    apt-get install -y --allow-unauthenticated libgdiplus && \
    apt-get install xvfb libfontconfig wkhtmltopdf libc6-dev openssl libssl-dev -y && \
    apt-get install libxrender1 libfontconfig1 libxext-dev -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /App
COPY ./Documents ./
COPY --from=build-env /App/out .
RUN mkdir uploads
ENV ASPNETCORE_URLS="http://+:5001"
ENTRYPOINT ["dotnet", "TaskManagerMiac.dll"]
