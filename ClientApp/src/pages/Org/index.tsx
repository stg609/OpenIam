import { PlusOutlined } from '@ant-design/icons';
import ProForm, { DrawerForm, ModalForm, ProFormSelect, ProFormSwitch, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumns } from '@ant-design/pro-table';
import type { ActionType as ProListActionType } from '@ant-design/pro-list/node_modules/@ant-design/pro-table';
import type { ReactText } from 'react';
import type { OrgDto } from './data';
import type { UserDto } from '../User/data';
import ProList from '@ant-design/pro-list';
import ProTable from '@ant-design/pro-table';
import ProCard from '@ant-design/pro-card';
import { Modal, Space, Button, Table, notification } from 'antd';
import React, { useRef, useState } from 'react';
import { useModel } from 'umi';
import * as RoleApi from '../../services/role';
import * as OrgApi from '../../services/org';
import * as UserApi from '../../services/user';

type OrgListItem = OrgDto;

export default (): React.ReactNode => {
    const { initialState } = useModel('@@initialState');
    const [ownedRoleRowKeys, setOwnedRoleRowKeys] = useState<ReactText[]>([]);
    const [unownedRoleRowKeys, setUnOwnedRoleRowKeys] = useState<ReactText[]>([]);
    const [ownedUserRowKeys, setOwnedUserRowKeys] = useState<ReactText[]>([]);
    const [unownedUserRowKeys, setUnOwnedUserRowKeys] = useState<ReactText[]>([]);
    const actionRef = useRef<ActionType>();
    const accessToken = initialState?.currentUser?.accessToken ?? "";
    const ownedRoleActionRef = useRef<ProListActionType>();
    const unownedRoleActionRef = useRef<ProListActionType>();
    const ownedUserActionRef = useRef<ProListActionType>();
    const unownedUserActionRef = useRef<ProListActionType>();

    const handleCreateAsync = async (fields: OrgListItem) => {
        const result = await OrgApi.createAsync(accessToken, fields);
        return result;
    }

    const handleBatchDeleteAsync = async (selectedKeys: React.ReactText[]) => {
        return await OrgApi.batchDeleteAsync(accessToken, selectedKeys.map(itm => itm.toString()));
    }

    const ownedRoleSelection = {
        selectedRowKeys: ownedRoleRowKeys,
        onChange: (keys: ReactText[]) => setOwnedRoleRowKeys(keys),
    };
    const unOwnedRoleSelection = {
        selectedRowKeys: unownedRoleRowKeys,
        onChange: (keys: ReactText[]) => setUnOwnedRoleRowKeys(keys),
    };
    const ownedUserSelection = {
        selectedRowKeys: ownedUserRowKeys,
        onChange: (keys: ReactText[]) => setOwnedUserRowKeys(keys),
    };
    const unOwnedUserSelection = {
        selectedRowKeys: unownedUserRowKeys,
        onChange: (keys: ReactText[]) => setUnOwnedUserRowKeys(keys),
    };
    const columns: ProColumns<OrgListItem>[] = [
        {
            title: '名称',
            dataIndex: 'name',
        },
        {
            title: '描述',
            hideInSearch: true,
            dataIndex: 'desc',
        },
        {
            title: '联系电话',
            hideInSearch: true,
            dataIndex: 'mobile',
        },
        {
            title: '联系地址',
            hideInSearch: true,
            dataIndex: 'address',
        },
        {
            title: '状态',
            hideInSearch: true,
            dataIndex: 'isEnabled',
            render: (_, record) => (
                record.isEnabled ? "已启用" : "未启用"
            )
        },
        {
            title: '创建时间',
            hideInSearch: true,
            dataIndex: 'createdAt',
            valueType: 'dateTime',
        },
        {
            title: '操作',
            fixed: 'right',
            valueType: 'option',
            render: (text, record) => [
                <ModalForm key="roles"
                    submitter={{
                        submitButtonProps: {
                            style: {
                                display: 'none',
                            },
                        },
                    }}
                    trigger={
                        <a>管理角色</a>
                    }>
                    <ProCard >
                        <ProCard bordered>
                            <ProList<API.RoleDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={ownedRoleActionRef}
                                size='small'
                                split={true}
                                headerTitle="已添加角色"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await OrgApi.getRolesAsync(accessToken, record.id);
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: '获取该组织的角色时遇到错误！', description: ex.message });
                                        return [];
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'name',
                                        title: '名称'
                                    }
                                }}
                                rowSelection={ownedRoleSelection}
                            />
                        </ProCard>
                        <ProCard
                            layout="center"
                            colSpan={{ sm: 4 }}
                            direction="column"
                        >
                            <ProCard type="inner" ghost={false} >
                                <Button onClick={async () => {
                                    try {
                                        await OrgApi.removeRolesAsync({ access_token: accessToken, id: record.id, ids: ownedRoleRowKeys.map(itm => itm.toString()) });
                                        ownedRoleActionRef.current?.reload()
                                        unownedRoleActionRef.current?.reload()
                                        setOwnedRoleRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '删除该组织机构的角色时遇到错误！', description: ex.message });
                                    }
                                }}>{'>>'}</Button>
                            </ProCard>
                            <ProCard type="inner" >
                                <Button onClick={async () => {
                                    try {
                                        await OrgApi.addRolesAsync({ access_token: accessToken, id: record.id, ids: unownedRoleRowKeys.map(itm => itm.toString()) });
                                        ownedRoleActionRef.current?.reload()
                                        unownedRoleActionRef.current?.reload()
                                        setUnOwnedRoleRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '为该组织机构增加角色时遇到错误！', description: ex.message });
                                    }
                                }}>{'<<'}</Button>
                            </ProCard>
                        </ProCard>
                        <ProCard bordered >
                            <ProList<API.RoleDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={unownedRoleActionRef}
                                size='small'
                                split={true}
                                headerTitle="待添加角色"
                                rowKey="id"
                                pagination={{
                                    defaultPageSize: 10,
                                    showSizeChanger: false,
                                }}
                                request={async (params) => {
                                    try {
                                        const result = await RoleApi.queryAsync({ access_token: accessToken, excludeOrgId: record.id, pageIndex: params.current, pageSize: params.pageSize });
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: "获取待添加角色时遇到错误", description: ex.message });
                                        return [];
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'name',
                                        title: '名称'
                                    }
                                }}
                                rowSelection={unOwnedRoleSelection}
                            />
                        </ProCard>
                    </ProCard>
                </ModalForm >,
                <ModalForm key="users"
                    submitter={{
                        submitButtonProps: {
                            style: {
                                display: 'none',
                            },
                        },
                    }}
                    trigger={
                        <a>管理用户</a>
                    }>
                    <ProCard >
                        <ProCard bordered>
                            <ProList<UserDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={ownedUserActionRef}
                                size='small'
                                split={true}
                                headerTitle="已添加用户"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await OrgApi.getUsersAsync(accessToken, record.id);
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: '获取该组织机构的用户时时遇到错误！', description: ex.message });
                                        return [];
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'username',
                                        title: '用户名'
                                    }
                                }}
                                rowSelection={ownedUserSelection}
                            />
                        </ProCard>
                        <ProCard
                            layout="center"
                            colSpan={{ sm: 4 }}
                            direction="column"
                        >
                            <ProCard type="inner" ghost={false} >
                                <Button onClick={async () => {
                                    try {
                                        await OrgApi.removeUsersAsync({ access_token: accessToken, id: record.id, ids: ownedUserRowKeys.map(itm => itm.toString()) });
                                        ownedUserActionRef.current?.reload()
                                        unownedUserActionRef.current?.reload()
                                        setUnOwnedUserRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '删除该组织机构的用户时遇到错误！', description: ex.message });
                                    }
                                }}>{'>>'}</Button>
                            </ProCard>
                            <ProCard type="inner" >
                                <Button onClick={async () => {
                                    try {
                                        await OrgApi.addUsersAsync({ access_token: accessToken, id: record.id, ids: unownedUserRowKeys.map(itm => itm.toString()) });
                                        ownedUserActionRef.current?.reload()
                                        unownedUserActionRef.current?.reload()
                                        setUnOwnedUserRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '增加该组织机构的用户时遇到错误！', description: ex.message });
                                    }
                                }}>{'<<'}</Button>
                            </ProCard>
                        </ProCard>
                        <ProCard bordered >
                            <ProList<UserDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={unownedUserActionRef}
                                size='small'
                                split={true}
                                headerTitle="待添加用户"
                                rowKey="id"
                                pagination={{
                                    defaultPageSize: 10,
                                    showSizeChanger: false,
                                }}
                                request={async (params) => {
                                    try {
                                        const result = await UserApi.queryAsync({ access_token: accessToken, excludeOrgId: record.id, pageIndex: params.current, pageSize: params.pageSize });
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: '查询待添加的用户列表失败！', description: ex.message });
                                        throw ex;
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'username',
                                        title: '用户名'
                                    }
                                }}
                                rowSelection={unOwnedUserSelection}
                            />
                        </ProCard>
                    </ProCard>
                </ModalForm>,
                <DrawerForm width="380px" key="detail"
                    title="详情"
                    initialValues={record}
                    trigger={
                        <a key="details">
                            详情
                        </a>
                    }
                    onFinish={async (value) => {
                        try {
                            await OrgApi.updateAsync(accessToken, record.id, value as OrgListItem);
                            if (actionRef.current) {
                                actionRef.current.reload();
                            }
                            notification.success({ message: "更新成功！" });
                            return true;
                        }
                        catch (ex) {
                            notification.error({ message: '更新组织机构时遇到错误！', description: ex.message });
                            return false;
                        }
                    }}
                >
                    <ProFormSelect
                        width="md"
                        name="parentId"
                        label="上级组织"
                        request={async () => {
                            try {
                                const orgs = await OrgApi.queryAsync(accessToken);
                                if (orgs.data) {
                                    return orgs.data.filter(itm => itm.id !== record.id).map(itm => ({ label: itm.name, value: itm.id }));
                                }

                                return [];
                            }
                            catch (ex) {
                                notification.error({ message: '获取上级组织机构列表时遇到错误！', description: ex.message });
                                return [];
                            }
                        }
                        }
                    />
                    <ProFormText
                        width="md"
                        name="name"
                        label="名称"
                        placeholder="请输入名称"
                        rules={[
                            {
                                required: true,
                                message: '名称',
                            }
                        ]}
                    />
                    <ProFormTextArea
                        width="md"
                        name="desc"
                        label="描述"
                        placeholder="请输入描述"
                    />
                    <ProFormText
                        width="md"
                        name="mobile"
                        label="联系电话"
                    />
                    <ProFormText
                        width="md"
                        name="address"
                        label="联系地址"
                    />
                    <ProFormSwitch
                        width="md"
                        name="isEnabled"
                        label="是否启用"
                    />
                </DrawerForm>
            ],
        }
    ];

    return (
        <PageContainer>
            <ProTable<OrgListItem>
                rowKey="id"
                columns={columns}
                actionRef={actionRef}
                request={async (params) => {
                    try {
                        const roles = await OrgApi.queryAsync(accessToken, params.name);
                        return roles;
                    }
                    catch (ex) {
                        notification.error({ message: '获取组织机构列表时遇到错误！', description: ex.message });
                        return [];
                    }
                }}
                rowSelection={{
                    // 自定义选择项参考: https://ant.design/components/table-cn/#components-table-demo-row-selection-custom
                    // 注释该行则默认不显示下拉选项
                    selections: [Table.SELECTION_ALL, Table.SELECTION_INVERT],
                }}
                tableAlertOptionRender={({ selectedRowKeys, onCleanSelected }) => {
                    return (
                        <Space size={16}>
                            <a onClick={async () => {
                                Modal.confirm({
                                    title: '操作',
                                    content: `确认要删除吗？`,
                                    okText: '确认',
                                    cancelText: '取消',
                                    onOk: async () => {
                                        try {
                                            await handleBatchDeleteAsync(selectedRowKeys);
                                            if (actionRef.current) {
                                                actionRef.current.reload();
                                            }
                                            onCleanSelected();
                                            notification.success({ message: "删除成功！" });
                                            return true;
                                        }
                                        catch (ex) {
                                            notification.error({ message: '删除组织机构时遇到错误！', description: ex.message });
                                            return false;
                                        }
                                    }
                                });

                            }}>批量删除</a>
                        </Space>
                    );
                }}
                toolBarRender={() => [
                    <ModalForm
                        width="380px"
                        title="新建机构"
                        trigger={
                            <Button type="primary">
                                <PlusOutlined />
                                新建机构
                            </Button>
                        }
                        onFinish={async (value) => {
                            try {
                                await handleCreateAsync(value as OrgListItem);
                                if (actionRef.current) {
                                    actionRef.current.reload();
                                }
                                notification.success({ message: "新建成功！" });
                                return true;
                            }
                            catch (ex) {
                                notification.error({ message: '组织机构创建失败！', description: ex.message });
                                return false;
                            }
                        }}
                    >
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="name"
                                label="名称"
                                placeholder="请输入名称"
                                rules={[
                                    {
                                        required: true,
                                        message: '请输入名称',
                                    }
                                ]}
                            />
                        </ProForm.Group>

                        <ProFormSelect
                            width="md"
                            name="parentId"
                            label="上级组织"
                            request={async () => {
                                try {
                                    const orgs = await OrgApi.queryAsync(accessToken);
                                    if (orgs.data) {
                                        return orgs.data.map(itm => ({ label: itm.name, value: itm.id }));
                                    }

                                    return [];
                                }
                                catch (ex) {
                                    notification.error({ message: '获取上级组织机构列表时遇到错误！', description: ex.message });
                                    return [];
                                }
                            }
                            }
                        />
                        <ProFormTextArea
                            name="desc"
                            label="描述"
                            placeholder="请输入描述"
                        />
                        <ProFormText
                            width="md"
                            name="mobile"
                            label="联系电话"
                        />
                        <ProFormText
                            width="md"
                            name="address"
                            label="联系地址"
                        />
                        <ProFormSwitch
                            width="md"
                            name="isEnabled"
                            label="是否启用"
                        />
                    </ModalForm>
                ]}
            />

        </PageContainer>
    )
};
