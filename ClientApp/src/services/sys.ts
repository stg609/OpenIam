import { PathBasePrefix } from "../../config/iamSettings";
import { ThinRequestAsync } from '@/utils/utils';
import type { SysDto } from '@/pages/Sys/data';

export async function queryAsync(access_token: string): Promise<SysDto> {

  const resp = await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/sys`,
  });

  return (await resp.json()) as SysDto;
}

export async function updateAsync(access_token: string, model: SysDto): Promise<void> {
  await ThinRequestAsync({
    access_token,
    url: `${PathBasePrefix}/admin/api/sys/`,
    method: 'put',
    body: JSON.stringify(model),
  });
}