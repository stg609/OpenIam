import { Button, Result } from 'antd';
import React from 'react';
import { history } from 'umi';

const InternalServerErrorPage: React.FC<{ location: any }> = (prop) => (
  <Result
    status="error"
    title="出错了！"
    subTitle={`很抱歉，似乎遇到了什么问题... （${prop.location.state.ex.message}）`}
    extra={
      <Button type="primary" onClick={() => history.push('/')}>
        Back Home
      </Button>
    }
  />
);

export default InternalServerErrorPage;
