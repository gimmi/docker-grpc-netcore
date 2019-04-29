Download certstrap [HERE](https://github.com/square/certstrap)

```
.\certstrap.exe init --common-name "MyCertAuth" --expires "100 years"
.\certstrap.exe request-cert --common-name "MyService"
.\certstrap.exe sign --CA "MyCertAuth" --expires "100 years" "MyService"
```