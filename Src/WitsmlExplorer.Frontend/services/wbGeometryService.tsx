import WbGeometryObject from "../models/wbGeometry";
import { ApiClient } from "./apiClient";

export default class WbGeometryObjectService {
  public static async getWbGeometrys(wellUid: string, wellboreUid: string, abortSignal?: AbortSignal): Promise<WbGeometryObject[]> {
    const response = await ApiClient.get(`/wells/${wellUid}/wellbores/${wellboreUid}/wbGeometrys`, abortSignal);
    if (response.ok) {
      return response.json();
    } else {
      return [];
    }
  }

  public static async getWbGeometry(wellUid: string, wellboreUid: string, wbGeometryId: string, abortSignal?: AbortSignal): Promise<WbGeometryObject> {
    const response = await ApiClient.get(`/wells/${wellUid}/wellbores/${wellboreUid}/wbGeometrys/${wbGeometryId}`, abortSignal);
    if (response.ok) {
      return response.json();
    } else {
      return null;
    }
  }
}
