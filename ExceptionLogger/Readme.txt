Method List
1. HappenException(Exception exception) : Exception 인자로 넘겨줘서 파일에 기록

2. ChangeLogFolderName(string name) : 로그 폴더명 수정(Default : Log)

3. ChangeLogFileName(string name) : 로그 파일명 수정 (Default : 없음)
- 예1) 파일명없음 : 20180302.log
- 예2) 파일명 'testException' 으로 지정 : testException_20180302.log

4. ChangeLogEncoding(string encodingName) : 로그 파일 기록시 문자셋 변경(Default : UTF8)
- ASCII, UTF8 중 하나만 가능