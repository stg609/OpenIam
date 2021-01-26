import React from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Card, Alert } from 'antd';
import { useIntl } from 'umi';

export default (): React.ReactNode => {
  const intl = useIntl();
  return (
    <PageContainer>
      <Card>
        <Alert
          message={intl.formatMessage({
            id: 'pages.welcome.alertMessage',
            defaultMessage: '欢迎使用SPA版本 OpenIam。',
          })}
          type="success"
          showIcon
          banner
          style={{
            margin: -12,
          }}
        />
       
      </Card>
    </PageContainer>
  );
};
