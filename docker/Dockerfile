FROM microsoft/dotnet:2.1.402-sdk-stretch

COPY binaries /opt/iap-bo-backend
COPY run.sh /usr/bin/iap-bo-backend-run.sh
RUN chmod +x /usr/bin/iap-bo-backend-run.sh

ENTRYPOINT ["bash"]
EXPOSE 8080
CMD ["-c", "iap-bo-backend-run.sh"]
