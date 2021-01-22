import React from 'react';
import { GithubOutlined } from '@ant-design/icons';
import { DefaultFooter } from '@ant-design/pro-layout';

export default () => (
  <DefaultFooter
    copyright="2021 OpenIam"
    links={[      
      {
        key: 'github',
        title: <GithubOutlined />,
        href: 'https://github.com/stg609/OpenIam',
        blankTarget: true,
      }
    ]}
  />
);
